using GawishERP.Application.Common.Posting;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;

namespace GawishERP.Infrastructure.Services;

public sealed class AccountResolver : IAccountResolver
{
    private readonly IPostingProfileRepository _postingProfileRepository;

    public AccountResolver(
        IPostingProfileRepository postingProfileRepository)
    {
        _postingProfileRepository = postingProfileRepository;
    }

    public async Task<IReadOnlyCollection<AccountResolutionResult>> ResolveAsync(
        PostingContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        //=========================================================
        // Resolve Posting Profile
        //=========================================================

        PostingProfile? profile;

        if (!string.IsNullOrWhiteSpace(context.PostingProfileCode))
        {
            profile =
                await _postingProfileRepository.GetByCodeAsync(
                    context.PostingProfileCode,
                    cancellationToken);
        }
        else
        {
            profile =
                await _postingProfileRepository.GetByDocumentTypeAsync(
                    context.DocumentType,
                    cancellationToken);
        }

        if (profile is null)
            return Array.Empty<AccountResolutionResult>();

        if (!profile.IsActive)
            throw new InvalidOperationException(
                $"Posting profile '{profile.Code}' is inactive.");

        //=========================================================
        // Validate Posting Profile
        //=========================================================

        if (!profile.Lines.Any())
        {
            throw new InvalidOperationException(
                $"Posting profile '{profile.Code}' has no posting lines.");
        }

        //=========================================================
        // Resolve Lines
        //=========================================================

        var results = new List<AccountResolutionResult>();

        foreach (var line in profile.Lines.OrderBy(x => x.Sequence))
        {
            if (line.AccountId == Guid.Empty)
            {
                throw new InvalidOperationException(
                    $"Posting profile '{profile.Code}' " +
                    $"line '{line.Sequence}' has no account.");
            }

            var amount = ResolveAmount(
                line.AmountSource,
                context);

            if (amount < 0)
            {
                throw new InvalidOperationException(
                    $"Posting amount cannot be negative. " +
                    $"Profile: '{profile.Code}', " +
                    $"Line: '{line.Sequence}'.");
            }

            if (amount == 0)
                continue;

            var calculatedAmount =
                amount * (line.Percentage / 100m);

            if (calculatedAmount == 0)
                continue;

            results.Add(
                new AccountResolutionResult
                {
                    AccountId = line.AccountId,
                    EntryType = line.EntryType,
                    Amount = calculatedAmount,
                    Description =
                        line.Description
                        ?? profile.Name
                        ?? context.Description
                        ?? context.DocumentType.ToString()
                });
        }

        //=========================================================
        // Validate Result
        //=========================================================

        if (!results.Any())
        {
            throw new InvalidOperationException(
                $"Posting profile '{profile.Code}' " +
                "resolved no posting entries.");
        }

        //=========================================================
        // Validate Debit / Credit Balance
        //=========================================================

        var totalDebit =
            results
                .Where(x => x.EntryType == Domain.Common.PostingEntryType.Debit)
                .Sum(x => x.Amount);

        var totalCredit =
            results
                .Where(x => x.EntryType == Domain.Common.PostingEntryType.Credit)
                .Sum(x => x.Amount);

        if (totalDebit != totalCredit)
        {
            throw new InvalidOperationException(
                $"Posting profile '{profile.Code}' is not balanced. " +
                $"Debit: {totalDebit}, Credit: {totalCredit}.");
        }

        return results;
    }

    //=========================================================
    // Amount Resolver
    //=========================================================

    private static decimal ResolveAmount(
        Domain.Common.PostingAmountSource amountSource,
        PostingContext context)
    {
        return amountSource switch
        {
            Domain.Common.PostingAmountSource.NetTotal =>
                context.Amount,

            Domain.Common.PostingAmountSource.TotalBeforeDiscount =>
                context.TotalBeforeDiscount,

            Domain.Common.PostingAmountSource.Discount =>
                context.DiscountAmount,

            Domain.Common.PostingAmountSource.Tax =>
                context.TaxAmount,

            Domain.Common.PostingAmountSource.Cost =>
                context.CostAmount,

            Domain.Common.PostingAmountSource.Quantity =>
                context.Quantity,

            Domain.Common.PostingAmountSource.Custom =>
                context.CustomAmount
                ?? throw new InvalidOperationException(
                    "Custom posting amount was requested " +
                    "but CustomAmount was not provided."),

            _ =>
                throw new InvalidOperationException(
                    $"Unsupported posting amount source: {amountSource}.")
        };
    }
}