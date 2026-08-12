using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Commands.Cancel;

public sealed class CancelPurchaseReturnCommandHandler
    : IRequestHandler<CancelPurchaseReturnCommand, CancelPurchaseReturnResponse>
{
    private readonly IPurchaseReturnRepository _purchaseReturnRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IFiscalYearRepository _fiscalYearRepository;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly ILedgerPostingService _ledgerPostingService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelPurchaseReturnCommandHandler(
        IPurchaseReturnRepository purchaseReturnRepository,
        IInventoryService inventoryService,
        IJournalEntryRepository journalEntryRepository,
        IFiscalYearRepository fiscalYearRepository,
        IDocumentNumberService documentNumberService,
        ILedgerPostingService ledgerPostingService,
        IUnitOfWork unitOfWork)
    {
        _purchaseReturnRepository = purchaseReturnRepository;
        _inventoryService = inventoryService;
        _journalEntryRepository = journalEntryRepository;
        _fiscalYearRepository = fiscalYearRepository;
        _documentNumberService = documentNumberService;
        _ledgerPostingService = ledgerPostingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelPurchaseReturnResponse> Handle(
        CancelPurchaseReturnCommand request,
        CancellationToken cancellationToken)
    {
        var purchaseReturn =
            await _purchaseReturnRepository.GetByIdWithLinesAsync(
                request.PurchaseReturnId,
                cancellationToken);

        if (purchaseReturn is null)
            throw new InvalidOperationException(
                "Purchase Return document not found.");

        if (purchaseReturn.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Purchase Return already cancelled.");

        if (purchaseReturn.Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Only posted Purchase Return can be cancelled.");

        // =========================================================
        // Accounting Reverse
        // =========================================================

        var originalJournal =
            await _journalEntryRepository.GetPostedByReferenceNumberAsync(
                purchaseReturn.DocumentNumber,
                DocumentType.PurchaseReturn,
                cancellationToken);

        if (originalJournal is null)
            throw new InvalidOperationException(
                $"Posted Purchase Return journal entry not found for {purchaseReturn.DocumentNumber}, or it has already been reversed.");

        var fiscalYear =
            await _fiscalYearRepository.GetByIdAsync(
                originalJournal.FiscalYearId);

        if (fiscalYear is null)
            throw new InvalidOperationException(
                "Fiscal Year for Purchase Return journal entry was not found.");

        if (!fiscalYear.IsOpen)
            throw new InvalidOperationException(
                "Fiscal Year is closed.");

        var reverseDocumentNumber =
            await _documentNumberService.GenerateAsync(
                DocumentType.JournalEntry,
                cancellationToken);

        var reverseJournal =
            originalJournal.CreateReverseEntry(reverseDocumentNumber);

        reverseJournal.Submit();
        reverseJournal.Approve();
        reverseJournal.Post();

        _journalEntryRepository.Add(reverseJournal);

        await _ledgerPostingService.PostAsync(
            reverseJournal,
            cancellationToken);

        originalJournal.MarkAsReversed(reverseJournal.Id);
        _journalEntryRepository.Update(originalJournal);

        // =========================================================
        // Inventory Reverse
        // =========================================================

        foreach (var line in purchaseReturn.Lines)
        {
            await _inventoryService.AddPurchaseAsync(
                line.ProductId,
                purchaseReturn.WarehouseId,
                line.Quantity,
                line.UnitCost,
                purchaseReturn.DocumentDate,
                purchaseReturn.Id,
                purchaseReturn.DocumentNumber,
                $"Reverse - {purchaseReturn.Notes}",
                cancellationToken);
        }

        purchaseReturn.Cancel();

        _purchaseReturnRepository.Update(purchaseReturn);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelPurchaseReturnResponse
        {
            Id = purchaseReturn.Id,
            DocumentNumber = purchaseReturn.DocumentNumber,
            Status = purchaseReturn.Status.ToString(),
            Message = "Purchase Return cancelled successfully."
        };
    }
}