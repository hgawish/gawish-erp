namespace GawishERP.Application.Features.Accounting.AccountStatement.DTOs;

public sealed class AccountStatementRowDto
{
    public DateTime PostingDate { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public string DocumentType { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal Debit { get; init; }

    public decimal Credit { get; init; }

    public decimal Balance { get; init; }
}