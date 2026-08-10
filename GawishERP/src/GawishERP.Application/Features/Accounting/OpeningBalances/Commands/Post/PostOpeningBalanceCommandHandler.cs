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
    private readonly IUnitOfWork _unitOfWork;

    public PostOpeningBalanceCommandHandler(
        IJournalEntryRepository journalEntryRepository,
        IUnitOfWork unitOfWork)
    {
        _journalEntryRepository = journalEntryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        PostOpeningBalanceCommand request,
        CancellationToken cancellationToken)
    {
        //=========================================================
        // Load Journal Entry WITH Lines
        //=========================================================

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

        //=========================================================
        // Continue Workflow According To Current Status
        //=========================================================

        switch (journalEntry.Status)
        {
            case DocumentStatus.Draft:

                // Draft → Submitted
                journalEntry.Submit();

                // Submitted → Approved
                journalEntry.Approve();

                // Approved → Posted
                journalEntry.Post();

                break;

            case DocumentStatus.Submitted:

                // Submitted → Approved
                journalEntry.Approve();

                // Approved → Posted
                journalEntry.Post();

                break;

            case DocumentStatus.Approved:

                // Approved → Posted
                journalEntry.Post();

                break;

            case DocumentStatus.Posted:

                return Result.Failure<Guid>(
                    new Error(
                        "OpeningBalance.AlreadyPosted",
                        "Opening Balance is already posted.",
                        ErrorType.Conflict));

            default:

                return Result.Failure<Guid>(
                    new Error(
                        "OpeningBalance.InvalidStatus",
                        $"Opening Balance cannot be posted from status '{journalEntry.Status}'.",
                        ErrorType.Validation));
        }

        //=========================================================
        // Save
        //=========================================================

        _journalEntryRepository.Update(journalEntry);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return Result.Success(journalEntry.Id);
    }
}