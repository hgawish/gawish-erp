using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Features.Accounting.JournalEntries.Commands.Reverse;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesReturn.Commands.Cancel;

public sealed class CancelSalesReturnHandler
    : IRequestHandler<CancelSalesReturnCommand, CancelSalesReturnResponse>
{
    private readonly ISalesReturnRepository _salesReturnRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSalesReturnHandler(
        ISalesReturnRepository salesReturnRepository,
        IJournalEntryRepository journalEntryRepository,
        IStockTransactionRepository stockTransactionRepository,
        IInventoryService inventoryService,
        IMediator mediator,
        IUnitOfWork unitOfWork)
    {
        _salesReturnRepository = salesReturnRepository;
        _journalEntryRepository = journalEntryRepository;
        _stockTransactionRepository = stockTransactionRepository;
        _inventoryService = inventoryService;
        _mediator = mediator;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelSalesReturnResponse> Handle(
        CancelSalesReturnCommand request,
        CancellationToken cancellationToken)
    {
        var salesReturn =
            await _salesReturnRepository.GetByIdWithLinesAsync(
                request.SalesReturnId,
                cancellationToken);

        if (salesReturn is null)
            throw new InvalidOperationException(
                "Sales return document not found.");

        if (salesReturn.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Sales return document already cancelled.");

        if (salesReturn.Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Only posted sales return documents can be cancelled.");

        //=====================================================
        // Find the original posted journal entry
        //=====================================================

        var journalEntry =
            await _journalEntryRepository
                .GetPostedByReferenceNumberAsync(
                    salesReturn.DocumentNumber,
                    DocumentType.SalesReturn,
                    cancellationToken);

        if (journalEntry is null)
            throw new InvalidOperationException(
                $"Posted journal entry was not found for sales return '{salesReturn.DocumentNumber}'.");

        if (journalEntry.IsReversed)
            throw new InvalidOperationException(
                "Sales return journal entry has already been reversed.");

        //=====================================================
        // Get the inventory transactions created by the
        // original Sales Return posting.
        //=====================================================
        //
        // Their UnitCost is the historical inventory cost used
        // when the return was posted. It is deliberately NOT
        // the customer's selling/refund price (UnitPrice).
        //=====================================================

        var salesReturnTransactions =
            await _stockTransactionRepository.GetByReferenceAsync(
                salesReturn.Id,
                StockTransactionType.SalesReturn);

        if (salesReturnTransactions.Count == 0)
            throw new InvalidOperationException(
                $"Sales return inventory transactions were not found for document '{salesReturn.DocumentNumber}'.");

        //=====================================================
        // Reverse Accounting
        //=====================================================
        //
        // Reuse the existing ReverseJournalEntryCommandHandler
        // instead of duplicating journal reversal logic here.
        //=====================================================

        var reverseResult =
            await _mediator.Send(
                new ReverseJournalEntryCommand(journalEntry.Id),
                cancellationToken);

        if (reverseResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Sales return journal entry could not be reversed. " +
                $"{reverseResult.Error.Code}: {reverseResult.Error.Message}");
        }

        //=====================================================
        // Reverse Inventory
        //=====================================================
        //
        // Sales Return originally increased inventory.
        // Cancellation therefore decreases inventory using
        // the SAME historical UnitCost that was recorded by the
        // original Sales Return stock transaction.
        //=====================================================

        foreach (var line in salesReturn.Lines)
        {
            var originalTransaction =
                salesReturnTransactions.FirstOrDefault(
                    x =>
                        x.ProductId == line.ProductId &&
                        x.WarehouseId == salesReturn.WarehouseId);

            if (originalTransaction is null)
                throw new InvalidOperationException(
                    $"Original sales return inventory transaction was not found for product '{line.ProductId}'.");

            if (originalTransaction.Quantity != line.Quantity)
                throw new InvalidOperationException(
                    $"Sales return inventory transaction quantity does not match document line for product '{line.ProductId}'.");

            await _inventoryService.ReverseSalesReturnAsync(
                line.ProductId,
                salesReturn.WarehouseId,
                line.Quantity,
                originalTransaction.UnitCost,
                salesReturn.DocumentDate,
                salesReturn.Id,
                salesReturn.DocumentNumber,
                salesReturn.Notes,
                cancellationToken);
        }

        //=====================================================
        // Cancel Sales Return
        //=====================================================

        salesReturn.Cancel();

        _salesReturnRepository.Update(salesReturn);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelSalesReturnResponse
        {
            Id = salesReturn.Id,
            DocumentNumber = salesReturn.DocumentNumber,
            Status = salesReturn.Status.ToString(),
            Message = "Sales return cancelled successfully."
        };
    }
}
