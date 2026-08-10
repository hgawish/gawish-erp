using GawishERP.Application.Features.FinancialReporting.Dtos;
using GawishERP.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Services;

public sealed partial class FinancialReportingService
{
    public async Task<GeneralLedgerDto> GetGeneralLedgerAsync(
        Guid accountId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var account = await _context.Accounts
            .AsNoTracking()
            .FirstAsync(x => x.Id == accountId, cancellationToken);

        var lines = await _context.JournalEntryLines
            .Include(x => x.JournalEntryHeader)
            .Where(x =>
                x.AccountId == accountId &&
                x.JournalEntryHeader.DocumentDate >= from &&
                x.JournalEntryHeader.DocumentDate <= to &&
                x.JournalEntryHeader.Status == DocumentStatus.Posted)
            .OrderBy(x => x.JournalEntryHeader.DocumentDate)
            .ThenBy(x => x.JournalEntryHeader.DocumentNumber)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        decimal runningBalance = 0m;

        var ledgerLines = new List<GeneralLedgerLineDto>();

        foreach (var line in lines)
        {
            runningBalance += line.Debit;
            runningBalance -= line.Credit;

            ledgerLines.Add(new GeneralLedgerLineDto
            {
                JournalEntryId = line.JournalEntryHeaderId,

                Date = line.JournalEntryHeader.DocumentDate,

                VoucherNo = line.JournalEntryHeader.DocumentNumber,

                Description = line.Description,

                Debit = line.Debit,

                Credit = line.Credit,

                RunningBalance = runningBalance
            });
        }

        return new GeneralLedgerDto
        {
            AccountId = account.Id,

            AccountCode = account.Code,

            AccountName = account.Name,

            From = from,

            To = to,

            OpeningBalance = 0m,

            ClosingBalance = runningBalance,

            Lines = ledgerLines
        };
    }
}