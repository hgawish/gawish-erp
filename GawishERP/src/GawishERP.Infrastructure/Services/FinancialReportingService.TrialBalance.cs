using GawishERP.Application.Features.Accounting.TrialBalance.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Services;

public sealed partial class FinancialReportingService
{
    public async Task<TrialBalanceReportDto> GetTrialBalanceAsync(
        DateTime asOfDate,
        CancellationToken cancellationToken = default)
    {
        var balances = await _context.AccountBalances
            .Include(x => x.Account)
            .AsNoTracking()
            .OrderBy(x => x.Account.Code)
            .ToListAsync(cancellationToken);

        var accounts = balances
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

        return new TrialBalanceReportDto
        {
            AsOfDate = asOfDate,

            TotalOpeningDebit =
                balances.Sum(x => x.OpeningDebit),

            TotalOpeningCredit =
                balances.Sum(x => x.OpeningCredit),

            TotalCurrentDebit =
                balances.Sum(x => x.CurrentDebit),

            TotalCurrentCredit =
                balances.Sum(x => x.CurrentCredit),

            TotalClosingBalance =
                balances.Sum(x => x.ClosingBalance),

            Accounts = accounts
        };
    }
}