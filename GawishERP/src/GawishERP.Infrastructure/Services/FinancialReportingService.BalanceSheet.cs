using GawishERP.Application.Features.FinancialReporting.Dtos;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Services;

public sealed partial class FinancialReportingService
{
    public async Task<BalanceSheetDto> GetBalanceSheetAsync(
        DateTime asOfDate,
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
                    .Sum(x => x.ClosingBalance)
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
}