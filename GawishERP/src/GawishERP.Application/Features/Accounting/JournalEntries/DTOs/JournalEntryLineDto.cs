namespace GawishERP.Application.Features.Accounting.JournalEntries.DTOs;

public sealed class JournalEntryLineDto
{
    public Guid Id { get; set; }

    public Guid AccountId { get; set; }

    public string AccountCode { get; set; } = string.Empty;

    public string AccountName { get; set; } = string.Empty;

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public string? Description { get; set; }
}