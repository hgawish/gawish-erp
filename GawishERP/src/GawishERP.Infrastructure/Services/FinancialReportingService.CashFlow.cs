using GawishERP.Application.Features.FinancialReporting.Dtos;
using GawishERP.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Services;

public sealed partial class FinancialReportingService
{
    public async Task<CashFlowDto> GetCashFlowAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var journalLines = await _context.JournalEntryLines
            .Include(x => x.Account)
            .Include(x => x.JournalEntryHeader)
            .Where(x =>
                x.JournalEntryHeader.Status == DocumentStatus.Posted &&
                x.JournalEntryHeader.DocumentDate >= from &&
                x.JournalEntryHeader.DocumentDate <= to &&
                x.Account.IsCashAccount)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        decimal operating = 0m;
        decimal investing = 0m;
        decimal financing = 0m;

        foreach (var line in journalLines)
        {
            var amount = line.Debit - line.Credit;

            switch (line.Account.AccountType)
            {
                case AccountType.Revenue:
                case AccountType.Expense:
                    operating += amount;
                    break;

                case AccountType.Asset:
                    investing += amount;
                    break;

                case AccountType.Liability:
                case AccountType.Equity:
                    financing += amount;
                    break;
            }
        }

        return new CashFlowDto
        {
            From = from,
            To = to,
            OperatingActivities = operating,
            InvestingActivities = investing,
            FinancingActivities = financing
        };
    }
}