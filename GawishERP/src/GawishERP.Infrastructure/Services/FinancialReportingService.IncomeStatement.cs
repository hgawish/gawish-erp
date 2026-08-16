using GawishERP.Application.Features.FinancialReporting.Dtos;
using GawishERP.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Services;

public sealed partial class FinancialReportingService
{
    public async Task<IncomeStatementDto> GetIncomeStatementAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        if (from > to)
            throw new ArgumentException("From date cannot be after To date.", nameof(from));

        var fiscalYear = await _context.FiscalYears
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.StartDate <= DateOnly.FromDateTime(from)
                    && x.EndDate >= DateOnly.FromDateTime(to),
                cancellationToken);

        if (fiscalYear is null)
            throw new InvalidOperationException(
                "No fiscal year covers the requested income statement period.");

        var nodes = await _context.FinancialStatementNodes
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

        // The source of truth for a financial statement is the posted
        // journal-entry lines. AccountBalances/LedgerTransactions may be
        // rebuilt asynchronously or may not contain historical data.
        //
        // Reversal handling:
        // - Original posted entries are excluded when IsReversed = true.
        // - Reversal entries are excluded when OriginalJournalEntryId != null.
        // This makes the report reflect the current business state rather
        // than counting both sides of a reversal pair.
        var lines = await _context.JournalEntryLines
            .Include(x => x.Account)
            .Include(x => x.JournalEntryHeader)
            .AsNoTracking()
            .Where(x =>
                x.JournalEntryHeader.FiscalYearId == fiscalYear.Id
                && x.JournalEntryHeader.DocumentDate >= from
                && x.JournalEntryHeader.DocumentDate <= to
                && x.JournalEntryHeader.Status == DocumentStatus.Posted
                && !x.JournalEntryHeader.IsReversed
                && x.JournalEntryHeader.OriginalJournalEntryId == null
                && x.Account.FinancialStatementNodeId != null)
            .ToListAsync(cancellationToken);

        var nodeDtos = nodes
            .Select(node => new FinancialStatementNodeDto
            {
                Id = node.Id,
                Code = node.Code,
                Name = node.Name,
                Level = node.Level,
                Amount = lines
                    .Where(x => x.Account.FinancialStatementNodeId == node.Id)
                    .Sum(x => GetIncomeStatementAmount(
                        x.Account.AccountType,
                        x.Debit,
                        x.Credit))
            })
            .ToList();

        var revenue = SumNode(nodeDtos, "IS-4");
        var costOfSales = SumNode(nodeDtos, "IS-5");
        var operatingExpenses = SumNode(nodeDtos, "IS-6");

        var grossProfit = revenue - costOfSales;
        var netProfit = grossProfit - operatingExpenses;

        return new IncomeStatementDto
        {
            From = from,
            To = to,
            Nodes = nodeDtos,
            Revenue = revenue,
            CostOfSales = costOfSales,
            GrossProfit = grossProfit,
            OperatingExpenses = operatingExpenses,
            NetProfit = netProfit
        };
    }

    private static decimal GetIncomeStatementAmount(
        AccountType accountType,
        decimal debit,
        decimal credit)
    {
        return accountType switch
        {
            // Revenue increases with credit and decreases with debit.
            // This also correctly handles contra-revenue accounts such as
            // Sales Returns, which are posted as debits.
            AccountType.Revenue => credit - debit,

            // Expense accounts increase with debit and decrease with credit.
            AccountType.Expense => debit - credit,

            _ => 0m
        };
    }
}
