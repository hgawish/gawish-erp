namespace GawishERP.Application.Features.Accounting.TrialBalance.DTOs;

public sealed class TrialBalanceRowDto
{
    public Guid AccountId { get; init; }

    public string AccountCode { get; init; } = string.Empty;

    public string AccountName { get; init; } = string.Empty;

    public decimal OpeningDebit { get; init; }

    public decimal OpeningCredit { get; init; }

    public decimal CurrentDebit { get; init; }

    public decimal CurrentCredit { get; init; }

    public decimal ClosingBalance { get; init; }
}