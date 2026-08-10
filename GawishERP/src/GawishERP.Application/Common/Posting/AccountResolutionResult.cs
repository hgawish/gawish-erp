using GawishERP.Domain.Common;

namespace GawishERP.Application.Common.Posting;

public sealed class AccountResolutionResult
{
    public Guid AccountId { get; init; }

    public PostingEntryType EntryType { get; init; }

    public decimal Amount { get; init; }

    public string Description { get; init; } = string.Empty;
}