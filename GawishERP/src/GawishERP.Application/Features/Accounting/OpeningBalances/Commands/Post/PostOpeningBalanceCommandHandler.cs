using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Results;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Post;

public sealed class PostOpeningBalanceCommandHandler
    : IRequestHandler<PostOpeningBalanceCommand, Result<Guid>>
{
    private readonly IJournalEntryRepository _journalEntryRepository;
    private readonly IFiscalYearRepository _fiscalYearRepository;
    private readonly ILedgerPostingService _ledgerPostingService;
    private readonly IUnitOfWork _unitOfWork;

    public PostOpeningBalanceCommandHandler(
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

    public async Task<Result<Guid>> Handle(
        PostOpeningBalanceCommand request,
        CancellationToken cancellationToken)
    {
        var journalEntry =
            await _journalEntryRepository.GetByIdWithLinesAsync(
                request.Id,
                cancellationToken);

        if (journalEntry is null)
        {
            return Result.Failure<Guid>(
                new Error(
                    "OpeningBalance.NotFound",
                    "Opening Balance journal entry was not found.",
                    ErrorType.NotFound));
        }

        if (journalEntry.Status == DocumentStatus.Posted)
        {
            return Result.Failure<Guid>(
                new Error(
                    "OpeningBalance.AlreadyPosted",
                    "Opening Balance is already posted.",
                    ErrorType.Conflict));
        }

        if (journalEntry.Status != DocumentStatus.Approved)
        {
            return Result.Failure<Guid>(
                new Error(
                    "OpeningBalance.NotApproved",
                    "Opening Balance must be approved before posting.",
                    ErrorType.Validation));
        }

        var fiscalYear =
            await _fiscalYearRepository.GetByIdAsync(
                journalEntry.FiscalYearId);

        if (fiscalYear is null)
        {
            return Result.Failure<Guid>(
                new Error(
                    "FiscalYear.NotFound",
                    "Fiscal Year was not found.",
                    ErrorType.NotFound));
        }

        if (!fiscalYear.IsOpen)
        {
            return Result.Failure<Guid>(
                new Error(
                    "FiscalYear.Closed",
                    "Fiscal Year is closed.",
                    ErrorType.Validation));
        }

        journalEntry.Post();

        await _ledgerPostingService.PostAsync(
            journalEntry,
            cancellationToken);

        _journalEntryRepository.Update(journalEntry);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Success(journalEntry.Id);
    }
}