using GawishERP.Domain.Entities;

namespace GawishERP.Application.Common.Interfaces;

public interface ILedgerPostingService
{
    Task PostAsync(
        JournalEntryHeader journalEntry,
        CancellationToken cancellationToken = default);
}