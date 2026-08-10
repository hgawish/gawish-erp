namespace GawishERP.Application.Features.FinancialReporting.Dtos;

public sealed class AccountStatementDto
{
    public Guid AccountId { get; init; }

    public string AccountCode { get; init; } = string.Empty;

    public string AccountName { get; init; } = string.Empty;

    public DateTime From { get; init; }

    public DateTime To { get; init; }

    public decimal OpeningBalance { get; init; }

    public decimal TotalDebit { get; init; }

    public decimal TotalCredit { get; init; }

    public decimal ClosingBalance { get; init; }

    public List<AccountStatementLineDto> Transactions { get; init; } = new();
}

public sealed class AccountStatementLineDto
{
    public Guid JournalEntryId { get; init; }

    public DateTime Date { get; init; }

    public string VoucherNo { get; init; } = string.Empty;

    public string ReferenceNo { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal Debit { get; init; }

    public decimal Credit { get; init; }

    public decimal Balance { get; init; }
}