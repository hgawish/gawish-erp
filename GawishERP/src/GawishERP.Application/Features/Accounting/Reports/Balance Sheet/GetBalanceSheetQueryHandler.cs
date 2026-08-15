using GawishERP.Application.Features.Accounting.BalanceSheet.DTOs;
using GawishERP.Application.Features.Accounting.BalanceSheet.Responses;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Accounting.BalanceSheet.Queries;

public sealed class GetBalanceSheetQueryHandler
    : IRequestHandler<GetBalanceSheetQuery, GetBalanceSheetResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly ILedgerTransactionRepository _ledgerRepository;

    public GetBalanceSheetQueryHandler(
        IAccountRepository accountRepository,
        ILedgerTransactionRepository ledgerRepository)
    {
        _accountRepository = accountRepository;
        _ledgerRepository = ledgerRepository;
    }

    public async Task<GetBalanceSheetResponse> Handle(
        GetBalanceSheetQuery request,
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

        var assets = new List<BalanceSheetRowDto>();
        var liabilities = new List<BalanceSheetRowDto>();
        var equity = new List<BalanceSheetRowDto>();
        decimal currentProfit = 0m;

        foreach (var account in accounts)
        {
            if (account.AccountType is not
                (AccountType.Asset or AccountType.Liability or AccountType.Equity or AccountType.Revenue or AccountType.Expense))
                continue;

            var transactions = await _ledgerRepository.GetAccountLedgerAsync(
                account.Id,
                request.FiscalYearId,
                null,
                null,
                request.CompanyId,
                request.BranchId,
                cancellationToken);

            var balance = account.Nature == AccountNature.Debit
                ? transactions.Sum(x => x.Debit - x.Credit)
                : transactions.Sum(x => x.Credit - x.Debit);

            if (account.AccountType == AccountType.Revenue)
            {
                currentProfit += transactions.Sum(x => x.Credit - x.Debit);
                continue;
            }

            if (account.AccountType == AccountType.Expense)
            {
                currentProfit -= transactions.Sum(x => x.Debit - x.Credit);
                continue;
            }

            if (balance == 0m)
                continue;

            var row = new BalanceSheetRowDto
            {
                AccountId = account.Id,
                AccountCode = account.Code,
                AccountName = account.Name,
                Balance = balance,
                Level = account.ParentAccountId.HasValue ? 1 : 0,
                IsHeader = !account.IsPostingAccount
            };

            switch (account.AccountType)
            {
                case AccountType.Asset:
                    assets.Add(row);
                    break;
                case AccountType.Liability:
                    liabilities.Add(row);
                    break;
                case AccountType.Equity:
                    equity.Add(row);
                    break;
            }
        }

        if (currentProfit != 0m)
        {
            equity.Add(new BalanceSheetRowDto
            {
                AccountId = Guid.Empty,
                AccountCode = "CURRENT-PROFIT",
                AccountName = "Current Period Profit",
                Balance = currentProfit,
                Level = 0,
                IsHeader = false
            });
        }

        var orderedAssets = assets.OrderBy(x => x.AccountCode).ToList();
        var orderedLiabilities = liabilities.OrderBy(x => x.AccountCode).ToList();
        var orderedEquity = equity.OrderBy(x => x.AccountCode).ToList();

        var totalAssets = orderedAssets.Sum(x => x.Balance);
        var totalLiabilities = orderedLiabilities.Sum(x => x.Balance);
        var totalEquity = orderedEquity.Sum(x => x.Balance);

        return new GetBalanceSheetResponse
        {
            Assets = new BalanceSheetSectionDto
            {
                Title = "Assets",
                Rows = orderedAssets,
                Total = totalAssets
            },
            Liabilities = new BalanceSheetSectionDto
            {
                Title = "Liabilities",
                Rows = orderedLiabilities,
                Total = totalLiabilities
            },
            Equity = new BalanceSheetSectionDto
            {
                Title = "Equity",
                Rows = orderedEquity,
                Total = totalEquity
            },
            TotalAssets = totalAssets,
            TotalLiabilities = totalLiabilities,
            TotalEquity = totalEquity
        };
    }
}
