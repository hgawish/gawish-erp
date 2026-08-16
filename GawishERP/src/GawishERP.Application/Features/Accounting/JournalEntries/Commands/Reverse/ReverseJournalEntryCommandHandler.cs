using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Results;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Commands.Reverse;

public sealed class ReverseJournalEntryCommandHandler
    : IRequestHandler<ReverseJournalEntryCommand, Result>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IFiscalYearRepository _fiscalYearRepository;
    private readonly IDocumentNumberService _documentNumberService;
    private readonly ILedgerPostingService _ledgerPostingService;
    private readonly IUnitOfWork _unitOfWork;

    public ReverseJournalEntryCommandHandler(
        IJournalEntryRepository journalEntryRepository,
        IFiscalYearRepository fiscalYearRepository,
        IDocumentNumberService documentNumberService,
        ILedgerPostingService ledgerPostingService,
        IUnitOfWork unitOfWork)
    {
        _journalEntryRepository = journalEntryRepository;
        _fiscalYearRepository = fiscalYearRepository;
        _documentNumberService = documentNumberService;
        _ledgerPostingService = ledgerPostingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ReverseJournalEntryCommand request,
        CancellationToken cancellationToken)
    {
        var original =
            await _journalEntryRepository.GetForReverseAsync(
                request.JournalEntryId,
                cancellationToken);

        if (original is null)
        {
            return Result.Failure(
                new Error(
                    "JournalEntry.NotFound",
                    "Journal Entry was not found.",
                    ErrorType.NotFound));
        }

        if (original.Status != DocumentStatus.Posted)
        {
            return Result.Failure(
                new Error(
                    "JournalEntry.NotPosted",
                    "Only posted journal entries can be reversed.",
                    ErrorType.Validation));
        }

        // A reversal is an accounting consequence of an original business
        // document. It must never become the source of another reversal.
        if (original.OriginalJournalEntryId.HasValue)
        {
            return Result.Failure(
                new Error(
                    "JournalEntry.ReversalCannotBeReversed",
                    "A reversal journal entry cannot be reversed again. Reverse the original business document instead.",
                    ErrorType.Validation));
        }

        if (original.IsReversed)
        {
            return Result.Failure(
                new Error(
                    "JournalEntry.AlreadyReversed",
                    "Journal Entry has already been reversed.",
                    ErrorType.Validation));
        }

        var fiscalYear =
            await _fiscalYearRepository.GetByIdAsync(
                original.FiscalYearId);

        if (fiscalYear is null)
        {
            return Result.Failure(
                new Error(
                    "FiscalYear.NotFound",
                    "Fiscal Year was not found.",
                    ErrorType.NotFound));
        }

        if (!fiscalYear.IsOpen)
        {
            return Result.Failure(
                new Error(
                    "FiscalYear.Closed",
                    "Fiscal Year is closed.",
                    ErrorType.Validation));
        }

        var documentNumber =
            await _documentNumberService.GenerateAsync(
                DocumentType.JournalEntry,
                cancellationToken);

        var reverseEntry =
            original.CreateReverseEntry(documentNumber);

        reverseEntry.Submit();
        reverseEntry.Approve();
        reverseEntry.Post();

        _journalEntryRepository.Add(reverseEntry);

        await _ledgerPostingService.PostAsync(
            reverseEntry,
            cancellationToken);

        original.MarkAsReversed(reverseEntry.Id);

        _journalEntryRepository.Update(original);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
