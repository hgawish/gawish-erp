using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.Accounts.Queries.GetById;

public sealed class GetAccountByIdHandler
    : IRequestHandler<GetAccountByIdQuery, GetAccountByIdResponse>
{
    private readonly IAccountRepository _accountRepository;

    public GetAccountByIdHandler(
        IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<GetAccountByIdResponse> Handle(
        GetAccountByIdQuery request,
        CancellationToken cancellationToken)
    {
        var account =
            await _accountRepository.GetByIdAsync(
                request.Id,
                cancellationToken);

        if (account is null)
            throw new InvalidOperationException(
                "Account not found.");

        return new GetAccountByIdResponse
        {
            Account = new AccountDto
            {
                Id = account.Id,

                Code = account.Code,

                Name = account.Name,

                ParentAccountId = account.ParentAccountId,

                ParentAccountCode = account.ParentAccount?.Code,

                ParentAccountName = account.ParentAccount?.Name,

                AccountType = account.AccountType.ToString(),

                Nature = account.Nature.ToString(),

                IsPostingAccount = account.IsPostingAccount,

                IsActive = account.IsActive
            }
        };
    }
}