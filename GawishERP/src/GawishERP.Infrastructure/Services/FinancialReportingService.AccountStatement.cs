using GawishERP.Application.Features.FinancialReporting.Dtos;

namespace GawishERP.Infrastructure.Services;

public sealed partial class FinancialReportingService
{
    public async Task<AccountStatementDto> GetAccountStatementAsync(
        Guid accountId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var ledger = await GetGeneralLedgerAsync(
            accountId,
            from,
            to,
            cancellationToken);

        return new AccountStatementDto
        {
            AccountId = ledger.AccountId,

            AccountCode = ledger.AccountCode,

            AccountName = ledger.AccountName,

            From = ledger.From,

            To = ledger.To,

            OpeningBalance = ledger.OpeningBalance,

            ClosingBalance = ledger.ClosingBalance,

            Transactions = ledger.Lines
                .Select(x => new AccountStatementLineDto
                {
                    Date = x.Date,

                    ReferenceNo = x.VoucherNo,

                    Description = x.Description,

                    Debit = x.Debit,

                    Credit = x.Credit,

                    Balance = x.RunningBalance
                })
                .ToList()
        };
    }
}