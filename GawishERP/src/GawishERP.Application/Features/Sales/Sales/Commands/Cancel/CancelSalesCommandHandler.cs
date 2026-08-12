using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Features.Accounting.JournalEntries.Commands.Reverse;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Commands.Cancel;

public sealed class CancelSalesCommandHandler
    : IRequestHandler<CancelSalesCommand, CancelSalesResponse>
{
    private readonly ISalesRepository _salesRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CancelSalesCommandHandler(
        ISalesRepository salesRepository,
        IInventoryService inventoryService,
        IStockTransactionRepository stockTransactionRepository,
        IJournalEntryRepository journalEntryRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _salesRepository = salesRepository;
        _inventoryService = inventoryService;
        _stockTransactionRepository = stockTransactionRepository;
        _journalEntryRepository = journalEntryRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<CancelSalesResponse> Handle(
        CancelSalesCommand request,
        CancellationToken cancellationToken)
    {
        var sales =
            await _salesRepository.GetByIdWithLinesAsync(
                request.SalesId,
                cancellationToken);

        if (sales is null)
            throw new InvalidOperationException(
                "Sales document not found.");

        if (sales.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Sales document already cancelled.");

        if (sales.Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Only posted sales can be cancelled.");

        // Find the original posted Sales journal before changing the document.
        var journalEntry = _journalEntryRepository
            .GetQueryable()
            .FirstOrDefault(x =>
                x.DocumentType == DocumentType.Sales &&
                x.ReferenceNumber == sales.DocumentNumber &&
                x.Status == DocumentStatus.Posted &&
                !x.IsReversed);

        if (journalEntry is null)
            throw new InvalidOperationException(
                $"Posted Sales journal entry not found for {sales.DocumentNumber}.");

        // Reverse inventory using the actual historical cost stored on the
        // original Sale stock transaction, never the sales UnitPrice.
        var originalSaleTransactions =
            await _stockTransactionRepository.GetByReferenceAsync(
                sales.Id,
                StockTransactionType.Sale);

        foreach (var line in sales.Lines)
        {
            var transactions = originalSaleTransactions
                .Where(x => x.ProductId == line.ProductId)
                .ToList();

            var totalOriginalQuantity = transactions.Sum(x => x.Quantity);

            if (transactions.Count == 0 || totalOriginalQuantity < line.Quantity)
            {
                throw new InvalidOperationException(
                    $"Original Sale stock transaction not found or insufficient for product {line.ProductId}.");
            }

            var weightedCost = transactions.Sum(x => x.Quantity * x.UnitCost);
            var historicalUnitCost =
                totalOriginalQuantity == 0
                    ? 0
                    : weightedCost / totalOriginalQuantity;

            var result = await _inventoryService.ReverseSaleAsync(
                line.ProductId,
                sales.WarehouseId,
                line.Quantity,
                historicalUnitCost,
                sales.DocumentDate,
                sales.Id,
                sales.DocumentNumber,
                sales.Notes,
                cancellationToken);

            if (result.Quantity != line.Quantity)
            {
                throw new InvalidOperationException(
                    "Inventory reversal quantity does not match the original sales line.");
            }
        }

        // Reverse the original Sales journal. The existing reverse workflow
        // creates and posts the opposite journal and marks the original as reversed.
        var reverseResult = await _mediator.Send(
            new ReverseJournalEntryCommand(journalEntry.Id),
            cancellationToken);

        if (reverseResult.IsFailure)
        {
            throw new InvalidOperationException(
                reverseResult.Error.Message);
        }

        sales.Cancel();

        _salesRepository.Update(sales);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelSalesResponse
        {
            Id = sales.Id,
            DocumentNumber = sales.DocumentNumber,
            Status = sales.Status.ToString(),
            Message = "Sales cancelled successfully."
        };
    }
}