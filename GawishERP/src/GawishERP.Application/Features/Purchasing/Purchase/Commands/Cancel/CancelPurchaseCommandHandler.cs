using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Features.Accounting.JournalEntries.Commands.Reverse;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Purchasing.Purchase.Commands.Cancel;

public sealed class CancelPurchaseCommandHandler
    : IRequestHandler<CancelPurchaseCommand, CancelPurchaseResponse>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public CancelPurchaseCommandHandler(
        IPurchaseRepository purchaseRepository,
        IInventoryService inventoryService,
        IStockTransactionRepository stockTransactionRepository,
        IJournalEntryRepository journalEntryRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _purchaseRepository = purchaseRepository;
        _inventoryService = inventoryService;
        _stockTransactionRepository = stockTransactionRepository;
        _journalEntryRepository = journalEntryRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<CancelPurchaseResponse> Handle(
        CancelPurchaseCommand request,
        CancellationToken cancellationToken)
    {
        var purchase =
            await _purchaseRepository.GetByIdWithLinesAsync(
                request.PurchaseId,
                cancellationToken);

        if (purchase is null)
            throw new InvalidOperationException(
                "Purchase document not found.");

        if (purchase.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Purchase document already cancelled.");

        if (purchase.Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Only posted purchase can be cancelled.");

        // Purchase posting stores the business document number in
        // JournalEntryHeader.DocumentNumber. The accounting ReferenceNumber
        // is the supplier invoice/reference number and therefore must not be
        // used to locate the original Purchase journal.
        var journalEntry = _journalEntryRepository
            .GetQueryable()
            .FirstOrDefault(x =>
                x.DocumentType == DocumentType.Purchase &&
                x.DocumentNumber == purchase.DocumentNumber &&
                x.Status == DocumentStatus.Posted &&
                !x.IsReversed);

        if (journalEntry is null)
            throw new InvalidOperationException(
                $"Posted Purchase journal entry not found for {purchase.DocumentNumber}, or it has already been reversed.");

        // Cancellation must use the historical cost recorded by the original
        // Purchase stock transactions, not a current inventory average and not
        // a recalculated document value.
        var originalPurchaseTransactions =
            await _stockTransactionRepository.GetByReferenceAsync(
                purchase.Id,
                StockTransactionType.Purchase);

        foreach (var line in purchase.Lines)
        {
            var transactions = originalPurchaseTransactions
                .Where(x => x.ProductId == line.ProductId)
                .ToList();

            var totalOriginalQuantity = transactions.Sum(x => x.Quantity);

            if (transactions.Count == 0 || totalOriginalQuantity < line.Quantity)
            {
                throw new InvalidOperationException(
                    $"Original Purchase stock transaction not found or insufficient for product {line.ProductId}.");
            }

            var weightedCost = transactions.Sum(x => x.Quantity * x.UnitCost);
            var historicalUnitCost =
                totalOriginalQuantity == 0
                    ? 0
                    : weightedCost / totalOriginalQuantity;

            var result = await _inventoryService.ReversePurchaseAsync(
                line.ProductId,
                purchase.WarehouseId,
                line.Quantity,
                historicalUnitCost,
                purchase.DocumentDate,
                purchase.Id,
                purchase.DocumentNumber,
                purchase.Notes,
                cancellationToken);

            if (result.Quantity != line.Quantity)
            {
                throw new InvalidOperationException(
                    "Inventory reversal quantity does not match the original purchase line.");
            }
        }

        // Reverse the original Purchase journal. This creates and posts the
        // opposite journal and marks the original journal as reversed.
        var reverseResult = await _mediator.Send(
            new ReverseJournalEntryCommand(journalEntry.Id),
            cancellationToken);

        if (reverseResult.IsFailure)
        {
            throw new InvalidOperationException(
                reverseResult.Error.Message);
        }

        purchase.Cancel();

        _purchaseRepository.Update(purchase);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelPurchaseResponse
        {
            Id = purchase.Id,
            DocumentNumber = purchase.DocumentNumber,
            Status = purchase.Status.ToString(),
            Message = "Purchase cancelled successfully."
        };
    }
}