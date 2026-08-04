namespace GawishERP.Application.Features.Accounting.OpeningBalances.DTOs;

public sealed class OpeningBalanceLineDto
{
    public Guid AccountId { get; set; }

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public string Description { get; set; } = string.Empty;
}