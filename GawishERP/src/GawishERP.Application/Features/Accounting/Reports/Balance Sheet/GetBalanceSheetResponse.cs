using GawishERP.Application.Features.Accounting.BalanceSheet.DTOs;

namespace GawishERP.Application.Features.Accounting.BalanceSheet.Responses;

public sealed class GetBalanceSheetResponse
{
    public BalanceSheetSectionDto Assets
        = new();

    public BalanceSheetSectionDto Liabilities
        = new();

    public BalanceSheetSectionDto Equity
        = new();

    public decimal TotalAssets { get; init; }

    public decimal TotalLiabilities { get; init; }

    public decimal TotalEquity { get; init; }

    public bool IsBalanced =>
        TotalAssets ==
        (TotalLiabilities + TotalEquity);
}