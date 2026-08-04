using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Results;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Cancel;

public sealed class CancelOpeningBalanceCommandHandler
    : IRequestHandler<CancelOpeningBalanceCommand, Result>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOpeningBalanceCommandHandler(
        IJournalEntryRepository journalEntryRepository,
        IUnitOfWork unitOfWork)
    {
        _journalEntryRepository = journalEntryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        CancelOpeningBalanceCommand request,
        CancellationToken cancellationToken)
    {
        var journalEntry =
            await _journalEntryRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

        if (journalEntry is null)
        {
            return Result.Failure(
                new Error(
                    "OpeningBalance.NotFound",
                    "Opening Balance not found.",
                    ErrorType.NotFound));
        }

        journalEntry.Cancel();

        _journalEntryRepository.Update(journalEntry);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}