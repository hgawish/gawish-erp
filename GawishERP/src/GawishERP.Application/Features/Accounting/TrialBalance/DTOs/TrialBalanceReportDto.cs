namespace GawishERP.Application.Features.Accounting.TrialBalance.DTOs;

public sealed class TrialBalanceReportDto
{
    public DateTime AsOfDate { get; init; }

    public decimal TotalOpeningDebit { get; init; }

    public decimal TotalOpeningCredit { get; init; }

    public decimal TotalCurrentDebit { get; init; }

    public decimal TotalCurrentCredit { get; init; }

    public decimal TotalClosingBalance { get; init; }

    public List<TrialBalanceDto> Accounts { get; init; } = [];
}