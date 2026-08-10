using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public sealed class Account : BaseEntity
{
    private readonly List<Account> _children = new();

    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public Guid? ParentAccountId { get; private set; }

    public AccountType AccountType { get; private set; }

    public AccountNature Nature { get; private set; }

    public bool IsPostingAccount { get; private set; }

    public bool IsActive { get; private set; }

    // NEW
    public bool IsCashAccount { get; private set; }

    // NEW
    public Guid? FinancialStatementNodeId { get; private set; }

    // Navigation

    public Account? ParentAccount { get; private set; }

    public IReadOnlyCollection<Account> Children
        => _children.AsReadOnly();

    public FinancialStatementNode? FinancialStatementNode { get; private set; }

    private Account()
    {
    }

    public Account(
        string code,
        string name,
        AccountType accountType,
        AccountNature nature,
        bool isPostingAccount,
        Guid? parentAccountId = null,
        Guid? financialStatementNodeId = null,
        bool isCashAccount = false)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(nameof(name));

        Code = code;
        Name = name;
        AccountType = accountType;
        Nature = nature;
        IsPostingAccount = isPostingAccount;
        ParentAccountId = parentAccountId;
        FinancialStatementNodeId = financialStatementNodeId;
        IsCashAccount = isCashAccount;

        IsActive = true;
    }

    public void Update(
        string name,
        bool isPostingAccount,
        Guid? parentAccountId,
        Guid? financialStatementNodeId,
        bool isCashAccount)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(nameof(name));

        Name = name;
        IsPostingAccount = isPostingAccount;
        ParentAccountId = parentAccountId;
        FinancialStatementNodeId = financialStatementNodeId;
        IsCashAccount = isCashAccount;
    }

    public void AssignFinancialStatementNode(Guid? nodeId)
    {
        FinancialStatementNodeId = nodeId;
    }

    public void SetCashAccount(bool value)
    {
        IsCashAccount = value;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}