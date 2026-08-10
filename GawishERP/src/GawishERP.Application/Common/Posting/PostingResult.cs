namespace GawishERP.Application.Common.Posting;

public sealed class PostingResult
{
    public Guid JournalEntryId { get; init; }

    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public IReadOnlyCollection<PostingResultLine> Lines { get; init; }
        = Array.Empty<PostingResultLine>();
}

public sealed class PostingResultLine
{
    public Guid AccountId { get; init; }

    public decimal Debit { get; init; }

    public decimal Credit { get; init; }

    public string Description { get; init; } = string.Empty;
}