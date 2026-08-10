using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public sealed class PostingProfile : BaseEntity
{
    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public DocumentType DocumentType { get; private set; }

    public Guid DebitAccountId { get; private set; }

    public Guid CreditAccountId { get; private set; }

    public CashFlowCategory CashFlowCategory { get; private set; }

    public bool IsActive { get; private set; }

    //=====================================
    // Navigation
    //=====================================

    public Account DebitAccount { get; private set; } = null!;

    public Account CreditAccount { get; private set; } = null!;

    //=====================================
    // Posting Profile Lines
    //=====================================

    private readonly List<PostingProfileLine> _lines = new();

    public IReadOnlyCollection<PostingProfileLine> Lines =>
        _lines.AsReadOnly();

    //=====================================
    // Constructor
    //=====================================

    private PostingProfile()
    {
    }

    public PostingProfile(
        string code,
        string name,
        DocumentType documentType,
        Guid debitAccountId,
        Guid creditAccountId,
        CashFlowCategory cashFlowCategory = CashFlowCategory.None)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Posting profile code is required.",
                nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Posting profile name is required.",
                nameof(name));

        if (debitAccountId == Guid.Empty)
            throw new ArgumentException(
                "Debit account is required.",
                nameof(debitAccountId));

        if (creditAccountId == Guid.Empty)
            throw new ArgumentException(
                "Credit account is required.",
                nameof(creditAccountId));

        Code = code.Trim();

        Name = name.Trim();

        DocumentType = documentType;

        DebitAccountId = debitAccountId;

        CreditAccountId = creditAccountId;

        CashFlowCategory = cashFlowCategory;

        IsActive = true;
    }

    //=====================================
    // Update
    //=====================================

    public void Update(
        string name,
        Guid debitAccountId,
        Guid creditAccountId,
        CashFlowCategory cashFlowCategory)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Posting profile name is required.",
                nameof(name));

        if (debitAccountId == Guid.Empty)
            throw new ArgumentException(
                "Debit account is required.",
                nameof(debitAccountId));

        if (creditAccountId == Guid.Empty)
            throw new ArgumentException(
                "Credit account is required.",
                nameof(creditAccountId));

        Name = name.Trim();

        DebitAccountId = debitAccountId;

        CreditAccountId = creditAccountId;

        CashFlowCategory = cashFlowCategory;
    }

    //=====================================
    // Posting Profile Lines
    //=====================================

    public void AddLine(
        PostingProfileLine line)
    {
        ArgumentNullException.ThrowIfNull(line);

        if (_lines.Any(x => x.Sequence == line.Sequence))
            throw new InvalidOperationException(
                $"Posting profile line sequence '{line.Sequence}' already exists.");

        _lines.Add(line);
    }

    public void RemoveLine(
        Guid lineId)
    {
        var line = _lines.FirstOrDefault(
            x => x.Id == lineId);

        if (line is null)
            return;

        _lines.Remove(line);
    }

    public void ClearLines()
    {
        _lines.Clear();
    }

    //=====================================
    // Cash Flow
    //=====================================

    public void SetCashFlowCategory(
        CashFlowCategory category)
    {
        CashFlowCategory = category;
    }

    //=====================================
    // Activation
    //=====================================

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}