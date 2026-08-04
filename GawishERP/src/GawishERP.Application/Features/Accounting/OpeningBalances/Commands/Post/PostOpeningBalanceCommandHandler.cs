using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Results;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Post;

public sealed class PostOpeningBalanceCommandHandler
    : IRequestHandler<PostOpeningBalanceCommand, Result>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly ILedgerTransactionRepository _ledgerTransactionRepository;
    private readonly ILedgerPostingService _ledgerPostingService;
    private readonly IUnitOfWork _unitOfWork;

    public PostOpeningBalanceCommandHandler(
        IJournalEntryRepository journalEntryRepository,
        ILedgerTransactionRepository ledgerTransactionRepository,
        ILedgerPostingService ledgerPostingService,
        IUnitOfWork unitOfWork)
    {
        _journalEntryRepository = journalEntryRepository;
        _ledgerTransactionRepository = ledgerTransactionRepository;
        _ledgerPostingService = ledgerPostingService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        PostOpeningBalanceCommand request,
        CancellationToken cancellationToken)
    {
        var journalEntry =
            await _journalEntryRepository.GetByIdWithLinesAsync(
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

        var alreadyPosted =
            await _ledgerTransactionRepository
                .ExistsForJournalEntryAsync(
                    journalEntry.Id,
                    cancellationToken);

        if (alreadyPosted)
        {
            return Result.Failure(
                new Error(
                    "OpeningBalance.AlreadyPosted",
                    "Opening Balance has already been posted.",
                    ErrorType.Conflict));
        }

        journalEntry.Post();

        await _ledgerPostingService.PostAsync(
            journalEntry,
            cancellationToken);

        _journalEntryRepository.Update(journalEntry);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}