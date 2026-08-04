using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Results;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Commands.Approve;

public sealed class ApproveJournalEntryCommandHandler
    : IRequestHandler<ApproveJournalEntryCommand, Result>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveJournalEntryCommandHandler(
        IJournalEntryRepository journalEntryRepository,
        IUnitOfWork unitOfWork)
    {
        _journalEntryRepository = journalEntryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ApproveJournalEntryCommand request,
        CancellationToken cancellationToken)
    {
        var journalEntry =
            await _journalEntryRepository.GetByIdAsync(
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

        if (journalEntry.Status != DocumentStatus.Submitted)
        {
            return Result.Failure(
                new Error(
                    "JournalEntry.InvalidStatus",
                    "Only Submitted journal entries can be approved.",
                    ErrorType.Validation));
        }

        journalEntry.Approve();

        _journalEntryRepository.Update(journalEntry);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}