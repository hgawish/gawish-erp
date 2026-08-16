using GawishERP.Application.Features.FinancialReporting.Dtos;
using GawishERP.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Services;

public sealed partial class FinancialReportingService
{
    public async Task<BalanceSheetDto> GetBalanceSheetAsync(
        DateTime asOfDate,
        CancellationToken cancellationToken = default)
    {
        var fiscalYear = await _context.FiscalYears
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.StartDate <= DateOnly.FromDateTime(asOfDate)
                    && x.EndDate >= DateOnly.FromDateTime(asOfDate),
                cancellationToken);

        if (fiscalYear is null)
            throw new InvalidOperationException(
                "No fiscal year covers the requested balance sheet date.");

        var nodes = await _context.FinancialStatementNodes
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

        // Opening balances are kept in AccountBalances, while all activity
        // after the fiscal-year opening is calculated from posted journal
        // entries. This keeps the balance sheet aligned with the P&L source
        // of truth and prevents stale/rebuilt transaction balances from
        // changing the report unexpectedly.
        var openingBalances = await _context.AccountBalances
            .AsNoTracking()
            .Where(x => x.FiscalYearId == fiscalYear.Id)
            .ToListAsync(cancellationToken);

        var lines = await _context.JournalEntryLines
            .Include(x => x.Account)
            .Include(x => x.JournalEntryHeader)
            .AsNoTracking()
            .Where(x =>
                x.JournalEntryHeader.FiscalYearId == fiscalYear.Id
                && x.JournalEntryHeader.DocumentDate <= asOfDate
                && x.JournalEntryHeader.Status == DocumentStatus.Posted
                && !x.JournalEntryHeader.IsReversed
                && x.JournalEntryHeader.OriginalJournalEntryId == null
                && x.Account.FinancialStatementNodeId != null
                && x.Account.AccountType != AccountType.Revenue
                && x.Account.AccountType != AccountType.Expense)
            .ToListAsync(cancellationToken);

        var accounts = await _context.Accounts
            .AsNoTracking()
            .Where(x => x.FinancialStatementNodeId != null)
            .ToListAsync(cancellationToken);

        var nodeDtos = nodes
            .Select(node => new FinancialStatementNodeDto
            {
                Id = node.Id,
                Code = node.Code,
                Name = node.Name,
                Level = node.Level,
                Amount = accounts
                    .Where(account => account.FinancialStatementNodeId == node.Id)
                    .Sum(account =>
                    {
                        var opening = openingBalances
                            .Where(x => x.AccountId == account.Id)
                            .Sum(x => x.OpeningDebit - x.OpeningCredit);

                        var activity = lines
                            .Where(x => x.AccountId == account.Id)
                            .Sum(x => x.Debit - x.Credit);

                        return GetBalanceSheetAmount(
                            account.AccountType,
                            opening + activity);
                    })
            })
            .ToList();

        return new BalanceSheetDto
        {
            AsOfDate = asOfDate,
            Nodes = nodeDtos,
            TotalAssets = SumNode(nodeDtos, "BS-1"),
            TotalLiabilities = SumNode(nodeDtos, "BS-2"),
            TotalEquity = SumNode(nodeDtos, "BS-3")
        };
    }

    private static decimal GetBalanceSheetAmount(
        AccountType accountType,
        decimal debitMinusCredit)
    {
        return accountType switch
        {
            AccountType.Asset => debitMinusCredit,
            AccountType.Liability => -debitMinusCredit,
            AccountType.Equity => -debitMinusCredit,
            _ => 0m
        };
    }
}
