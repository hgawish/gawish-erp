namespace GawishERP.Application.Features.Accounting.GeneralLedger.DTOs;

public sealed class GeneralLedgerRowDto
{
    public DateTime PostingDate { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public string DocumentType { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public decimal Debit { get; init; }

    public decimal Credit { get; init; }

    public decimal RunningBalance { get; init; }
}