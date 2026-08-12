using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IJournalEntryRepository
{
    void Add(JournalEntryHeader journalEntry);

    void Update(JournalEntryHeader journalEntry);

    Task<JournalEntryHeader?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<JournalEntryHeader?> GetByIdWithLinesAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<JournalEntryHeader?> GetByIdForViewAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<JournalEntryHeader?> GetForReverseAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<JournalEntryHeader?> GetPostedByReferenceNumberAsync(
        string referenceNumber,
        DocumentType documentType,
        CancellationToken cancellationToken = default);

    Task<(List<JournalEntryHeader> Items, int TotalCount)> GetAllAsync(
        string? search,
        DateTime? fromDate,
        DateTime? toDate,
        DocumentStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<(List<JournalEntryHeader> Items, int TotalCount)> GetOpeningBalancesAsync(
        string? search,
        DateTime? fromDate,
        DateTime? toDate,
        DocumentStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsOpeningBalanceAsync(
        Guid fiscalYearId,
        Guid? companyId,
        Guid? branchId,
        CancellationToken cancellationToken = default);

    IQueryable<JournalEntryHeader> GetQueryable();
}