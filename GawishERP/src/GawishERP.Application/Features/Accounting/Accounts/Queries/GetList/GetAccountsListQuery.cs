using MediatR;

namespace GawishERP.Application.Features.Accounting.Accounts.Queries.GetList;

public sealed record GetAccountsListQuery(
    string? Search,
    Guid? ParentAccountId,
    bool? IsPostingAccount,
    bool? IsActive,
    int PageNumber = 1,
    int PageSize = 20)
    : IRequest<GetAccountsListResponse>;