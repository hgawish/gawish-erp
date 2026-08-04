using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Results;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Approve;

public sealed class ApproveOpeningBalanceCommandHandler
    : IRequestHandler<ApproveOpeningBalanceCommand, Result>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveOpeningBalanceCommandHandler(
        IJournalEntryRepository journalEntryRepository,
        IUnitOfWork unitOfWork)
    {
        _journalEntryRepository = journalEntryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        ApproveOpeningBalanceCommand request,
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

        journalEntry.Approve();

        _journalEntryRepository.Update(journalEntry);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}