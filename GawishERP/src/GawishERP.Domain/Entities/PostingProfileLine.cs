using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public sealed class PostingProfileLine : BaseEntity
{
    public Guid PostingProfileId { get; private set; }

    public int Sequence { get; private set; }

    public PostingEntryType EntryType { get; private set; }

    public Guid AccountId { get; private set; }

    public PostingAmountSource AmountSource { get; private set; }

    public decimal Percentage { get; private set; }

    public string? Description { get; private set; }

    // ==========================
    // Navigation
    // ==========================

    public PostingProfile PostingProfile { get; private set; } = null!;

    public Account Account { get; private set; } = null!;

    private PostingProfileLine()
    {
    }

    public PostingProfileLine(
        int sequence,
        PostingEntryType entryType,
        Guid accountId,
        PostingAmountSource amountSource,
        decimal percentage = 100m,
        string? description = null)
    {
        if (sequence <= 0)
            throw new ArgumentException(nameof(sequence));

        if (accountId == Guid.Empty)
            throw new ArgumentException(nameof(accountId));

        if (percentage <= 0)
            throw new ArgumentException(nameof(percentage));

        Sequence = sequence;

        EntryType = entryType;

        AccountId = accountId;

        AmountSource = amountSource;

        Percentage = percentage;

        Description = description;
    }

    public void Update(
        PostingEntryType entryType,
        Guid accountId,
        PostingAmountSource amountSource,
        decimal percentage,
        string? description)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException(nameof(accountId));

        EntryType = entryType;

        AccountId = accountId;

        AmountSource = amountSource;

        Percentage = percentage;

        Description = description;
    }
}