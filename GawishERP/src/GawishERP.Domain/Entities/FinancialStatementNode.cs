using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public sealed class FinancialStatementNode : BaseEntity
{
    private readonly List<FinancialStatementNode> _children = new();

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public FinancialStatementType StatementType { get; private set; }

    public Guid? ParentId { get; private set; }

    public NormalBalance NormalBalance { get; private set; }

    public int SortOrder { get; private set; }

    public bool IsSystem { get; private set; }

    public bool IsEditable { get; private set; }

    // Navigation

    public FinancialStatementNode? Parent { get; private set; }

    public IReadOnlyCollection<FinancialStatementNode> Children
        => _children.AsReadOnly();

    private FinancialStatementNode()
    {
    }

    public FinancialStatementNode(
        string code,
        string name,
        FinancialStatementType statementType,
        NormalBalance normalBalance,
        int sortOrder,
        bool isSystem,
        bool isEditable,
        Guid? parentId = null)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(nameof(name));

        Code = code;

        Name = name;

        StatementType = statementType;

        NormalBalance = normalBalance;

        SortOrder = sortOrder;

        IsSystem = isSystem;

        IsEditable = isEditable;

        ParentId = parentId;
    }

    public void Update(
        string name,
        int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(nameof(name));

        Name = name;

        SortOrder = sortOrder;
    }

    public void ChangeParent(Guid? parentId)
    {
        ParentId = parentId;
    }

    public void SetEditable(bool editable)
    {
        IsEditable = editable;
    }
}