using GawishERP.Application.Features.FinancialReporting.Dtos;

namespace GawishERP.Infrastructure.Services;

public sealed partial class FinancialReportingService
{
    private static decimal SumNode(
        IEnumerable<FinancialStatementNodeDto> nodes,
        string prefix)
    {
        return nodes
            .Where(x => x.Code.StartsWith(prefix))
            .Sum(x => x.Amount);
    }
}