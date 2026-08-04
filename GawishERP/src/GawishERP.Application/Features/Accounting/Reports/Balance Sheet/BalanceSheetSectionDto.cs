namespace GawishERP.Application.Features.Accounting.BalanceSheet.DTOs;

public sealed class BalanceSheetSectionDto
{
    public string Title { get; init; } = string.Empty;

    public IReadOnlyList<BalanceSheetRowDto> Rows
        = new List<BalanceSheetRowDto>();

    public decimal Total { get; init; }
}