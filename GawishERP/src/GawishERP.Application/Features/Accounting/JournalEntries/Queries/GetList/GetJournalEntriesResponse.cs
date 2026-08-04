using GawishERP.Application.Features.Accounting.JournalEntries.DTOs;

namespace GawishERP.Application.Features.Accounting.JournalEntries.Queries.GetList;

public sealed class GetJournalEntriesResponse
{
    public int TotalCount { get; set; }

    public List<JournalEntryDto> Items { get; set; } = new();
}