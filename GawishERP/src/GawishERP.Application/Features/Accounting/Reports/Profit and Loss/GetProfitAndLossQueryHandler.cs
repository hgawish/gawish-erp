using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using GawishERP.Application.Features.Accounting.Reports.Profit_and_Loss.DTOs;
using MediatR;

namespace GawishERP.Application.Features.Accounting.Reports.Profit_and_Loss;

public sealed class GetProfitAndLossQueryHandler
    : IRequestHandler<GetProfitAndLossQuery, GetProfitAndLossResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILedgerTransactionRepository _ledgerRepository;

    public GetProfitAndLossQueryHandler(
        IAccountRepository accountRepository,
        ILedgerTransactionRepository ledgerRepository)
    {
        _accountRepository = accountRepository;
        _ledgerRepository = ledgerRepository;
    }

    public async Task<GetProfitAndLossResponse> Handle(
        GetProfitAndLossQuery request,
        CancellationToken cancellationToken)
    {
        var (accounts, _) = await _accountRepository.GetAllAsync(
            search: null,
            parentAccountId: null,
            isPostingAccount: true,
            isActive: true,
            pageNumber: 1,
            pageSize: 10000,
            cancellationToken);

        var revenue = new List<ProfitAndLossLineDto>();
        var expenses = new List<ProfitAndLossLineDto>();

        foreach (var account in accounts)
        {
            if (account.AccountType is not (AccountType.Revenue or AccountType.Expense))
                continue;

            var transactions = await _ledgerRepository.GetAccountLedgerAsync(
                account.Id,
                request.FiscalYearId,
                request.FromDate,
                request.ToDate,
                request.CompanyId,
                request.BranchId,
                cancellationToken);

            if (transactions.Count == 0)
                continue;

            var amount = account.AccountType == AccountType.Revenue
                ? transactions.Sum(x => x.Credit - x.Debit)
                : transactions.Sum(x => x.Debit - x.Credit);

            if (amount == 0m)
                continue;

            var line = new ProfitAndLossLineDto
            {
                AccountId = account.Id,
                AccountCode = account.Code,
                AccountName = account.Name,
                Amount = amount
            };

            if (account.AccountType == AccountType.Revenue)
                revenue.Add(line);
            else
                expenses.Add(line);
        }

        var totalRevenue = revenue.Sum(x => x.Amount);
        var totalExpenses = expenses.Sum(x => x.Amount);

        return new GetProfitAndLossResponse
        {
            FiscalYearId = request.FiscalYearId,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            Revenue = revenue.OrderBy(x => x.AccountCode).ToList(),
            Expenses = expenses.OrderBy(x => x.AccountCode).ToList(),
            TotalRevenue = totalRevenue,
            TotalExpenses = totalExpenses,
            NetProfit = totalRevenue - totalExpenses
        };
    }
}
