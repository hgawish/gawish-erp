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
    private readonly IInventoryService _inventoryService;
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSalesReturnHandler(
        ISalesReturnRepository salesReturnRepository,
        IJournalEntryRepository journalEntryRepository,
        IInventoryService inventoryService,
        IMediator mediator,
        IUnitOfWork unitOfWork)
    {
        _salesReturnRepository = salesReturnRepository;
        _journalEntryRepository = journalEntryRepository;
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
        // Reverse Accounting
        //=====================================================
        //
        // Reuse the existing ReverseJournalEntryCommandHandler
        // instead of duplicating journal reversal logic here.
        //
        // The existing handler guarantees that a posted journal
        // entry cannot be reversed more than once.
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
        // the same historical unit cost.
        //=====================================================

        foreach (var line in salesReturn.Lines)
        {
            await _inventoryService.ReverseSalesReturnAsync(
                line.ProductId,
                salesReturn.WarehouseId,
                line.Quantity,
                line.UnitPrice,
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