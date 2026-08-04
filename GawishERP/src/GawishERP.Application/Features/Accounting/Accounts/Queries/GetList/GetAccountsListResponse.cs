namespace GawishERP.Application.Features.Accounting.Accounts.Queries.GetList;

public sealed class GetAccountsListResponse
{
    public List<AccountListItemDto> Items { get; set; } = new();

    public int TotalCount { get; set; }

    public int PageNumber { get; set; }

    public int PageSize { get; set; }
}