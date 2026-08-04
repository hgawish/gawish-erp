namespace GawishERP.Application.Features.Accounting.Accounts.Queries.GetList;

public sealed class AccountListItemDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? ParentAccountCode { get; set; }

    public string? ParentAccountName { get; set; }

    public string AccountType { get; set; } = string.Empty;

    public string Nature { get; set; } = string.Empty;

    public bool IsPostingAccount { get; set; }

    public bool IsActive { get; set; }
}