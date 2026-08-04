using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Results;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Commands.Submit;

public sealed class SubmitJournalEntryCommandHandler
    : IRequestHandler<SubmitJournalEntryCommand, Result>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitJournalEntryCommandHandler(
        IJournalEntryRepository journalEntryRepository,
        IUnitOfWork unitOfWork)
    {
        _journalEntryRepository = journalEntryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        SubmitJournalEntryCommand request,
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

        if (journalEntry.Status != DocumentStatus.Draft)
        {
            return Result.Failure(
                new Error(
                    "JournalEntry.InvalidStatus",
                    "Only Draft journal entries can be submitted.",
                    ErrorType.Validation));
        }

        journalEntry.Submit();

        _journalEntryRepository.Update(journalEntry);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}