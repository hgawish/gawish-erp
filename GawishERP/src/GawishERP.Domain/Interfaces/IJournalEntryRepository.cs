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

    /// <summary>
    /// يستخدم أثناء الـ Reverse لضمان تحميل كل البيانات المطلوبة
    /// </summary>
    Task<JournalEntryHeader?> GetForReverseAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// جميع القيود
    /// </summary>
    Task<(List<JournalEntryHeader> Items, int TotalCount)> GetAllAsync(
        string? search,
        DateTime? fromDate,
        DateTime? toDate,
        DocumentStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// قيود الأرصدة الافتتاحية فقط
    /// </summary>
    Task<(List<JournalEntryHeader> Items, int TotalCount)> GetOpeningBalancesAsync(
        string? search,
        DateTime? fromDate,
        DateTime? toDate,
        DocumentStatus? status,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// التحقق من وجود Opening Balance
    /// لنفس السنة المالية والشركة والفرع.
    /// يسمح النظام بمستند Opening Balance واحد فقط
    /// لكل FiscalYear + Company + Branch.
    /// </summary>
    Task<bool> ExistsOpeningBalanceAsync(
        Guid fiscalYearId,
        Guid? companyId,
        Guid? branchId,
        CancellationToken cancellationToken = default);

    IQueryable<JournalEntryHeader> GetQueryable();
}