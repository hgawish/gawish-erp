using GawishERP.Application.Features.FinancialReporting.Dtos;
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

        var transactions = await _context.LedgerTransactions
            .Include(x => x.Account)
            .AsNoTracking()
            .Where(x =>
                x.FiscalYearId == fiscalYear.Id
                && x.PostingDate >= from
                && x.PostingDate <= to
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
            // Revenue accounts always contribute according to the
            // financial-statement effect of their normal balance.
            // Regular revenue (Credit nature): Credit - Debit.
            // Contra-revenue such as Sales Returns (Debit nature):
            // its debit balance must reduce revenue, so the same
            // Credit - Debit expression correctly produces a negative amount.
            GawishERP.Domain.Common.AccountType.Revenue
                => credit - debit,

            // Expense accounts increase with debits.
            GawishERP.Domain.Common.AccountType.Expense
                => debit - credit,

            _ => 0m
        };
    }
}
