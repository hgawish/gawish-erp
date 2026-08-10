namespace GawishERP.Application.Features.FinancialReporting.Dtos;

public sealed class CashFlowDto
{
    public DateTime From { get; init; }

    public DateTime To { get; init; }

    public decimal OperatingActivities { get; init; }

    public decimal InvestingActivities { get; init; }

    public decimal FinancingActivities { get; init; }

    public decimal NetCashFlow =>
        OperatingActivities
        + InvestingActivities
        + FinancingActivities;
}