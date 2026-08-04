using GawishERP.Application.Features.Accounting.TrialBalance.DTOs;

namespace GawishERP.Application.Features.Accounting.TrialBalance.Responses;

public sealed class GetTrialBalanceResponse
{
    public IReadOnlyList<TrialBalanceDto> Accounts { get; init; }
        = new List<TrialBalanceDto>();

    public decimal TotalOpeningDebit { get; init; }

    public decimal TotalOpeningCredit { get; init; }

    public decimal TotalCurrentDebit { get; init; }

    public decimal TotalCurrentCredit { get; init; }

    public decimal TotalClosingDebit { get; init; }

    public decimal TotalClosingCredit { get; init; }
}