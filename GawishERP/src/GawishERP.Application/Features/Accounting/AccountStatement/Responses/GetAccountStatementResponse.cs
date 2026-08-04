using GawishERP.Application.Features.Accounting.AccountStatement.DTOs;

namespace GawishERP.Application.Features.Accounting.AccountStatement.Responses;

public sealed class GetAccountStatementResponse
{
    public Guid AccountId { get; init; }

    public string AccountCode { get; init; } = string.Empty;

    public string AccountName { get; init; } = string.Empty;

    public decimal OpeningBalance { get; init; }

    public IReadOnlyList<AccountStatementRowDto> Transactions
        = new List<AccountStatementRowDto>();

    public decimal TotalDebit { get; init; }

    public decimal TotalCredit { get; init; }

    public decimal ClosingBalance { get; init; }
}