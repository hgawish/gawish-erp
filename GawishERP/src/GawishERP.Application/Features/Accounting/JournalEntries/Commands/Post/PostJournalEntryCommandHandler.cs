using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Results;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Commands.Post;

public sealed class PostJournalEntryCommandHandler
    : IRequestHandler<PostJournalEntryCommand, Result>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IFiscalYearRepository _fiscalYearRepository;
    private readonly ILedgerPostingService _ledgerPostingService;
    private readonly IUnitOfWork _unitOfWork;

    public PostJournalEntryCommandHandler(
        IJournalEntryRepository journalEntryRepository,
        IFiscalYearRepository fiscalYearRepository,
        ILedgerPostingService ledgerPostingService,
        IUnitOfWork unitOfWork)
    {
        _journalEntryRepository = journalEntryRepository;
        _fiscalYearRepository = fiscalYearRepository;
        _ledgerPostingService = ledgerPostingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        PostJournalEntryCommand request,
        CancellationToken cancellationToken)
    {
        var journalEntry =
            await _journalEntryRepository.GetByIdWithLinesAsync(
                request.JournalEntryId,
                cancellationToken);

        if (journalEntry is null)
        {
            return Result.Failure(
                new Error(
                    "JournalEntry.NotFound",
                    "Journal Entry was not found.",
                    ErrorType.NotFound));
        }

        if (journalEntry.Status == DocumentStatus.Posted)
        {
            return Result.Failure(
                new Error(
                    "JournalEntry.AlreadyPosted",
                    "Journal Entry is already posted.",
                    ErrorType.Validation));
        }

        // لا يسمح بالترحيل إلا بعد الاعتماد
        if (journalEntry.Status != DocumentStatus.Approved)
        {
            return Result.Failure(
                new Error(
                    "JournalEntry.NotApproved",
                    "Journal Entry must be approved before posting.",
                    ErrorType.Validation));
        }

        var fiscalYear =
            await _fiscalYearRepository.GetByIdAsync(
                journalEntry.FiscalYearId);

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

        // تغيير الحالة إلى Posted
        journalEntry.Post();

        // ترحيل القيود إلى الأستاذ العام
        await _ledgerPostingService.PostAsync(
            journalEntry,
            cancellationToken);

        _journalEntryRepository.Update(journalEntry);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}