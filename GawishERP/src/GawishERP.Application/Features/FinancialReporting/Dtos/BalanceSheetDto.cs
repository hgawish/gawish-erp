namespace GawishERP.Application.Features.FinancialReporting.Dtos;

public sealed class BalanceSheetDto
{
    public DateTime AsOfDate { get; set; }

    public decimal TotalAssets { get; set; }

    public decimal TotalLiabilities { get; set; }

    public decimal TotalEquity { get; set; }

    public List<FinancialStatementNodeDto> Nodes { get; set; } = [];
}