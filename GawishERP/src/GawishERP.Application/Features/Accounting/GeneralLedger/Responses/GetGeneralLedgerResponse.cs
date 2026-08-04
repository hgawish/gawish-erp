using GawishERP.Application.Features.Accounting.GeneralLedger.DTOs;

namespace GawishERP.Application.Features.Accounting.GeneralLedger.Responses;

public sealed class GetGeneralLedgerResponse
{
    public Guid AccountId { get; init; }

    public string AccountCode { get; init; } = string.Empty;

    public string AccountName { get; init; } = string.Empty;

    public decimal OpeningBalance { get; init; }

    public IReadOnlyList<GeneralLedgerRowDto> Transactions { get; init; }
        = new List<GeneralLedgerRowDto>();

    public decimal TotalDebit { get; init; }

    public decimal TotalCredit { get; init; }

    public decimal ClosingBalance { get; init; }
}