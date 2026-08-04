namespace GawishERP.Application.Features.Accounting.JournalEntries.DTOs;

public sealed class CreateJournalEntryDto
{
    public DateOnly JournalDate { get; set; }

    public Guid FiscalYearId { get; set; }

    public Guid? CompanyId { get; set; }

    public Guid? BranchId { get; set; }

    public string ReferenceNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<CreateJournalEntryLineDto> Lines { get; set; } = new();
}