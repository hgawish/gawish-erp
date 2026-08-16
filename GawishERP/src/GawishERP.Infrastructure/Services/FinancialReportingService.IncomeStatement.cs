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
        var nodes = await _context.FinancialStatementNodes
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ToListAsync(cancellationToken);

        var balances = await _context.AccountBalances
            .Include(x => x.Account)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var nodeDtos = nodes
            .Select(node => new FinancialStatementNodeDto
            {
                Id = node.Id,
                Code = node.Code,
                Name = node.Name,
                Level = node.Level,

                Amount = balances
                    .Where(x =>
                        x.Account.FinancialStatementNodeId == node.Id)
                    .Sum(x => x.CurrentCredit - x.CurrentDebit)
            })
            .ToList();

        var revenue = SumNode(nodeDtos, "IS-4");

        var costOfSales = SumNode(nodeDtos, "IS-5");

        var operatingExpenses = SumNode(nodeDtos, "IS-6");

        return new IncomeStatementDto
        {
            From = from,

            To = to,

            Nodes = nodeDtos,

            Revenue = revenue,

            CostOfSales = costOfSales,

            GrossProfit = revenue - costOfSales,

            OperatingExpenses = operatingExpenses,

            NetProfit = revenue
                        - costOfSales
                        - operatingExpenses
        };
    }
}