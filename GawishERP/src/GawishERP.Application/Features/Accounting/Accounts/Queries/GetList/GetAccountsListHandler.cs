using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.Accounts.Queries.GetList;

public sealed class GetAccountsListHandler
    : IRequestHandler<GetAccountsListQuery, GetAccountsListResponse>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountsListHandler(
        IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<GetAccountsListResponse> Handle(
        GetAccountsListQuery request,
        CancellationToken cancellationToken)
    {
        var (items, totalCount) =
            await _accountRepository.GetAllAsync(
                request.Search,
                request.ParentAccountId,
                request.IsPostingAccount,
                request.IsActive,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

        return new GetAccountsListResponse
        {
            Items = items
                .Select(x => new AccountListItemDto
                {
                    Id = x.Id,

                    Code = x.Code,

                    Name = x.Name,

                    ParentAccountCode = x.ParentAccount?.Code,

                    ParentAccountName = x.ParentAccount?.Name,

                    AccountType = x.AccountType.ToString(),

                    Nature = x.Nature.ToString(),

                    IsPostingAccount = x.IsPostingAccount,

                    IsActive = x.IsActive
                })
                .ToList(),

            TotalCount = totalCount,

            PageNumber = request.PageNumber,

            PageSize = request.PageSize
        };
    }
}