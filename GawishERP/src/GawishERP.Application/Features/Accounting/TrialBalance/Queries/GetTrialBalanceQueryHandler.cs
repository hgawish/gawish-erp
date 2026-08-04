using GawishERP.Application.Features.Accounting.TrialBalance.DTOs;
using GawishERP.Application.Features.Accounting.TrialBalance.Responses;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.TrialBalance.Queries;

public sealed class GetTrialBalanceQueryHandler
    : IRequestHandler<GetTrialBalanceQuery, GetTrialBalanceResponse>
{
    private readonly IAccountBalanceRepository _accountBalanceRepository;

    public GetTrialBalanceQueryHandler(
        IAccountBalanceRepository accountBalanceRepository)
    {
        _accountBalanceRepository = accountBalanceRepository;
    }

    public async Task<GetTrialBalanceResponse> Handle(
        GetTrialBalanceQuery request,
        CancellationToken cancellationToken)
    {
        var balances =
            await _accountBalanceRepository.GetTrialBalanceAsync(
                request.FiscalYearId,
                request.CompanyId,
                request.BranchId);

        var rows = balances
    .Select(x => new TrialBalanceDto
    {
                AccountId = x.AccountId,

                AccountCode = x.Account.Code,

                AccountName = x.Account.Name,

                OpeningDebit = x.OpeningDebit,

                OpeningCredit = x.OpeningCredit,

                CurrentDebit = x.CurrentDebit,

                CurrentCredit = x.CurrentCredit,

                ClosingBalance = x.ClosingBalance
            })
            .ToList();

        var response = new GetTrialBalanceResponse
        {
            Accounts = rows,

            TotalOpeningDebit =
                rows.Sum(x => x.OpeningDebit),

            TotalOpeningCredit =
                rows.Sum(x => x.OpeningCredit),

            TotalCurrentDebit =
                rows.Sum(x => x.CurrentDebit),

            TotalCurrentCredit =
                rows.Sum(x => x.CurrentCredit),

            TotalClosingDebit =
                rows.Where(x => x.ClosingBalance > 0)
                    .Sum(x => x.ClosingBalance),

            TotalClosingCredit =
                rows.Where(x => x.ClosingBalance < 0)
                    .Sum(x => Math.Abs(x.ClosingBalance))
        };

        return response;
    }
}