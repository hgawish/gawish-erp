using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class JournalEntryLine : BaseEntity
{
    public Guid JournalEntryHeaderId { get; private set; }

    public Guid AccountId { get; private set; }

    public decimal Debit { get; private set; }

    public decimal Credit { get; private set; }

    public string Description { get; private set; } = string.Empty;

    // Navigation

    public JournalEntryHeader JournalEntryHeader { get; private set; } = null!;

    public Account Account { get; private set; } = null!;

    private JournalEntryLine()
    {
    }

    public JournalEntryLine(
        Guid accountId,
        decimal debit,
        decimal credit,
        string description)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException(nameof(accountId));

        if (debit < 0)
            throw new ArgumentException(nameof(debit));

        if (credit < 0)
            throw new ArgumentException(nameof(credit));

        if (debit == 0 && credit == 0)
            throw new InvalidOperationException(
                "Debit and Credit cannot both be zero.");

        if (debit > 0 && credit > 0)
            throw new InvalidOperationException(
                "Line cannot contain both Debit and Credit.");

        AccountId = accountId;

        Debit = debit;

        Credit = credit;

        Description = description;
    }
}