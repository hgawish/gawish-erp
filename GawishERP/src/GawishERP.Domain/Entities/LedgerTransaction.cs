using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public sealed class LedgerTransaction : BaseEntity
{
    public Guid JournalEntryHeaderId { get; private set; }

    public Guid JournalEntryLineId { get; private set; }

    public Guid AccountId { get; private set; }

    public Guid FiscalYearId { get; private set; }

    public Guid? CompanyId { get; private set; }

    public Guid? BranchId { get; private set; }

    public DateTime PostingDate { get; private set; }

    public string DocumentNumber { get; private set; } = string.Empty;

    public DocumentType DocumentType { get; private set; }

    public decimal Debit { get; private set; }

    public decimal Credit { get; private set; }

    /// <summary>
    /// Running balance after this transaction.
    /// Calculated during posting.
    /// </summary>
    public decimal RunningBalance { get; private set; }

    public string Description { get; private set; } = string.Empty;

    // Navigation

    public JournalEntryHeader JournalEntryHeader { get; private set; } = null!;

    public JournalEntryLine JournalEntryLine { get; private set; } = null!;

    public Account Account { get; private set; } = null!;

    public FiscalYear FiscalYear { get; private set; } = null!;

    private LedgerTransaction()
    {
    }

    public LedgerTransaction(
        Guid journalEntryHeaderId,
        Guid journalEntryLineId,
        Guid accountId,
        Guid fiscalYearId,
        Guid? companyId,
        Guid? branchId,
        DateTime postingDate,
        string documentNumber,
        DocumentType documentType,
        decimal debit,
        decimal credit,
        decimal runningBalance,
        string description)
    {
        if (accountId == Guid.Empty)
            throw new ArgumentException(nameof(accountId));

        if (journalEntryHeaderId == Guid.Empty)
            throw new ArgumentException(nameof(journalEntryHeaderId));

        if (journalEntryLineId == Guid.Empty)
            throw new ArgumentException(nameof(journalEntryLineId));

        JournalEntryHeaderId = journalEntryHeaderId;

        JournalEntryLineId = journalEntryLineId;

        AccountId = accountId;

        FiscalYearId = fiscalYearId;

        CompanyId = companyId;

        BranchId = branchId;

        PostingDate = postingDate;

        DocumentNumber = documentNumber;

        DocumentType = documentType;

        Debit = debit;

        Credit = credit;

        RunningBalance = runningBalance;

        Description = description;
    }
}