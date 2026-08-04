namespace GawishERP.Application.Features.Accounting.BalanceSheet.DTOs;

public sealed class BalanceSheetRowDto
{
    public Guid AccountId { get; init; }

    public string AccountCode { get; init; } = string.Empty;

    public string AccountName { get; init; } = string.Empty;

    public decimal Balance { get; init; }

    public int Level { get; init; }

    public bool IsHeader { get; init; }
}