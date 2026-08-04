namespace GawishERP.Application.Features.Accounting.JournalEntries.DTOs;

public sealed class CreateJournalEntryLineDto
{
    public Guid AccountId { get; set; }

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public string? Description { get; set; }
}