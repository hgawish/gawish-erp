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

        // LedgerTransactions are immutable posting records. When a posted
        // journal entry is reversed, both the original entry and its reversal
        // remain in the ledger. For financial statements we must report the
        // current business state, not both sides of a reversal pair.
        //
        // Therefore:
        // 1. The original entry must not be IsReversed.
        // 2. A reversal entry (OriginalJournalEntryId != null) must not be
        //    included independently.
        //
        // This also prevents the historical "Reverse - Reverse" test data
        // from changing the financial result when the report is rebuilt.
        var transactions = await _context.LedgerTransactions
            .Include(x => x.Account)
            .Include(x => x.JournalEntryHeader)
            .AsNoTracking()
            .Where(x =>
                x.FiscalYearId == fiscalYear.Id
                && x.PostingDate >= from
                && x.PostingDate <= to
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
                Amount = transactions
                    .Where(x => x.Account.FinancialStatementNodeId == node.Id)
                    .Sum(x => GetIncomeStatementAmount(
                        x.Account.AccountType,
                        x.Account.Nature,
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
        GawishERP.Domain.Common.AccountType accountType,
        GawishERP.Domain.Common.AccountNature nature,
        decimal debit,
        decimal credit)
    {
        return accountType switch
        {
            // Revenue accounts increase with credits.
            // Contra-revenue accounts such as Sales Returns increase with
            // debits, so the same Credit - Debit expression reduces revenue.
            GawishERP.Domain.Common.AccountType.Revenue
                => credit - debit,

            // Expense accounts increase with debits.
            GawishERP.Domain.Common.AccountType.Expense
                => debit - credit,

            _ => 0m
        };
    }
}
