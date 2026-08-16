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

        // The journal is the accounting source of truth for financial reports.
        // Do NOT remove reversed journal entries here.
        //
        // A reversal is itself a posted accounting transaction and therefore
        // must participate in the report:
        //
        //   Original + Reverse                 => net zero
        //   Original + Reverse + Reverse again => original effect restored
        //
        // Filtering IsReversed / OriginalJournalEntryId would hide these
        // transactions and can produce incorrect financial statements.
        //
        // When To is supplied at midnight (for example 2026-08-15T00:00:00),
        // treat it as the end of that calendar day. When a real time is
        // supplied, use that exact timestamp as the exclusive upper boundary.
        var toExclusive =
            to.TimeOfDay == TimeSpan.Zero
                ? to.Date.AddDays(1)
                : to;

        var lines = await _context.JournalEntryLines
            .Include(x => x.Account)
            .Include(x => x.JournalEntryHeader)
            .AsNoTracking()
            .Where(x =>
                x.JournalEntryHeader.FiscalYearId == fiscalYear.Id
                && x.JournalEntryHeader.DocumentDate >= from
                && x.JournalEntryHeader.DocumentDate < toExclusive
                && x.JournalEntryHeader.Status == DocumentStatus.Posted
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
            // Revenue increases by credit and decreases by debit.
            // This naturally handles contra-revenue accounts such as
            // Sales Returns because their return entries are debits.
            AccountType.Revenue => credit - debit,

            // Expenses increase by debit and decrease by credit.
            AccountType.Expense => debit - credit,

            _ => 0m
        };
    }
}
