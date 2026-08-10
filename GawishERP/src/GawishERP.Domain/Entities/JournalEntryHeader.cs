using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class JournalEntryHeader : BaseDocumentEntity
{
    private readonly List<JournalEntryLine> _lines = new();
    public DocumentType DocumentType { get; private set; }

    public string ReferenceNumber { get; private set; } = string.Empty;

    public decimal TotalDebit { get; private set; }

    public decimal TotalCredit { get; private set; }

    /// <summary>
    /// القيد الأصلى الذى تم عمل Reverse له
    /// </summary>
    public Guid? OriginalJournalEntryId { get; private set; }

    /// <summary>
    /// القيد العكسى الذى قام بعكس هذا القيد
    /// </summary>
    public Guid? ReversedByJournalEntryId { get; private set; }

    public bool IsReversed { get; private set; }

    //========================
    // Navigation
    //========================

    public JournalEntryHeader? OriginalJournalEntry { get; private set; }

    public JournalEntryHeader? ReversedByJournalEntry { get; private set; }

    public IReadOnlyCollection<JournalEntryLine> Lines =>
        _lines.AsReadOnly();

    private JournalEntryHeader()
    {
    }

    public JournalEntryHeader(
        string documentNumber,
        DateTime documentDate,
        Guid fiscalYearId,
        DocumentType documentType,
        string referenceNumber,
        string? notes,
        Guid? companyId = null,
        Guid? branchId = null)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new ArgumentException(nameof(documentNumber));

        if (fiscalYearId == Guid.Empty)
            throw new ArgumentException(nameof(fiscalYearId));

        DocumentNumber = documentNumber;

        DocumentDate = documentDate;

        FiscalYearId = fiscalYearId;

        CompanyId = companyId;

        BranchId = branchId;

        DocumentType = documentType;

        ReferenceNumber = referenceNumber;

        Notes = notes;

        Status = DocumentStatus.Draft;
    }

    public void AddLine(
        Guid accountId,
        decimal debit,
        decimal credit,
        string description)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Cannot modify journal entry unless it is Draft.");

        var line = new JournalEntryLine(
            accountId,
            debit,
            credit,
            description);

        _lines.Add(line);

        Recalculate();
    }

    public void RemoveLine(Guid lineId)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Cannot modify journal entry unless it is Draft.");

        var line = _lines.FirstOrDefault(x => x.Id == lineId);

        if (line is null)
            return;

        _lines.Remove(line);

        Recalculate();
    }

    private void Recalculate()
    {
        TotalDebit = _lines.Sum(x => x.Debit);

        TotalCredit = _lines.Sum(x => x.Credit);
    }

    public override void Submit()
    {
        if (!_lines.Any())
            throw new InvalidOperationException(
                "Journal entry has no lines.");

        Recalculate();

        if (TotalDebit != TotalCredit)
            throw new InvalidOperationException(
                "Journal entry is not balanced.");

        base.Submit();
    }

    public override void Approve()
    {
        if (Status != DocumentStatus.Submitted)
            throw new InvalidOperationException(
                "Only submitted journal entries can be approved.");

        base.Approve();
    }

    public override void Post()
    {
        if (Status != DocumentStatus.Approved)
            throw new InvalidOperationException(
                "Only approved journal entries can be posted.");

        if (!_lines.Any())
            throw new InvalidOperationException(
                "Journal entry has no lines.");

        Recalculate();

        if (TotalDebit != TotalCredit)
            throw new InvalidOperationException(
                "Journal entry is not balanced.");

        base.Post();
    }

    public override void Cancel()
    {
        base.Cancel();
    }

    /// <summary>
    /// يربط القيد الجديد بالقيد الأصلى
    /// </summary>
    public void MarkAsReverseOf(Guid originalJournalEntryId)
    {
        OriginalJournalEntryId = originalJournalEntryId;
    }

    /// <summary>
    /// تعليم القيد بأنه تم عمل Reverse له
    /// </summary>
    public void MarkAsReversed(Guid reverseJournalEntryId)
    {
        if (Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Only posted journal entries can be reversed.");

        if (IsReversed)
            throw new InvalidOperationException(
                "Journal entry has already been reversed.");

        IsReversed = true;

        ReversedByJournalEntryId = reverseJournalEntryId;
    }

    /// <summary>
    /// إنشاء القيد العكسى
    /// </summary>
    public JournalEntryHeader CreateReverseEntry(
        string documentNumber)
    {
        if (Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Only posted journal entries can be reversed.");

        if (IsReversed)
            throw new InvalidOperationException(
                "Journal entry has already been reversed.");

        var reverseEntry = new JournalEntryHeader(
            documentNumber,
            DateTime.UtcNow,
            FiscalYearId,
            DocumentType,
            DocumentNumber,
            $"Reverse of {DocumentNumber}",
            CompanyId,
            BranchId);

        reverseEntry.MarkAsReverseOf(Id);

        foreach (var line in _lines)
        {
            reverseEntry.AddLine(
                line.AccountId,
                line.Credit,
                line.Debit,
                $"Reverse - {line.Description}");
        }

        return reverseEntry;
    }
}