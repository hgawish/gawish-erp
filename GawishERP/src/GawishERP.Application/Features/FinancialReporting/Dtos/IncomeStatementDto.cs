namespace GawishERP.Application.Features.FinancialReporting.Dtos;

public sealed class IncomeStatementDto
{
    public DateTime From { get; set; }

    public DateTime To { get; set; }

    public decimal Revenue { get; set; }

    public decimal CostOfSales { get; set; }

    public decimal GrossProfit { get; set; }

    public decimal OperatingExpenses { get; set; }

    public decimal NetProfit { get; set; }

    public List<FinancialStatementNodeDto> Nodes { get; set; } = [];
}