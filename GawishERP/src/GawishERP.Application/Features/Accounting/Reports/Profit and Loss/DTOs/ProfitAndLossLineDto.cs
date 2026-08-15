namespace GawishERP.Application.Features.Accounting.Reports.Profit_and_Loss.DTOs;

public sealed class ProfitAndLossLineDto
{
    public Guid AccountId { get; init; }
    public string AccountCode { get; init; } = string.Empty;
    public string AccountName { get; init; } = string.Empty;
    public decimal Amount { get; init; }
}
