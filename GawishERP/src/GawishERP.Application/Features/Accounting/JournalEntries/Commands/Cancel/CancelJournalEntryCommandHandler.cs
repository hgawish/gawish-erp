using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Results;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Commands.Cancel;

public sealed class CancelJournalEntryCommandHandler
    : IRequestHandler<CancelJournalEntryCommand, Result>
{
    private readonly IJournalEntryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelJournalEntryCommandHandler(
        IJournalEntryRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        CancelJournalEntryCommand request,
        CancellationToken cancellationToken)
    {
        var journal =
            await _repository.GetByIdAsync(
                request.JournalEntryId,
                cancellationToken);

        if (journal is null)
        {
            return Result.Failure(
                new Error(
                    "JournalEntry.NotFound",
                    "Journal Entry was not found.",
                    ErrorType.NotFound));
        }

        journal.Cancel();

        _repository.Update(journal);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}