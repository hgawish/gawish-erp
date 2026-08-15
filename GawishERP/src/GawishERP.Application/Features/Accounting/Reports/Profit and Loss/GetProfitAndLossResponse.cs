using GawishERP.Application.Features.Accounting.Reports.Profit_and_Loss.DTOs;

namespace GawishERP.Application.Features.Accounting.Reports.Profit_and_Loss;

public sealed class GetProfitAndLossResponse
{
    public Guid FiscalYearId { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public IReadOnlyList<ProfitAndLossLineDto> Revenue { get; init; } = [];
    public IReadOnlyList<ProfitAndLossLineDto> Expenses { get; init; } = [];
    public decimal TotalRevenue { get; init; }
    public decimal TotalExpenses { get; init; }
    public decimal NetProfit { get; init; }
}
