using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface ILedgerTransactionRepository
{
    void Add(LedgerTransaction transaction);

    void AddRange(IEnumerable<LedgerTransaction> transactions);

    Task<bool> ExistsForJournalEntryAsync(
        Guid journalEntryHeaderId,
        CancellationToken cancellationToken = default);

    Task<List<LedgerTransaction>> GetByJournalEntryAsync(
        Guid journalEntryHeaderId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// جميع حركات حساب معين خلال فترة.
    /// يستخدم فى:
    /// - General Ledger
    /// - Account Statement
    /// </summary>
    Task<List<LedgerTransaction>> GetAccountLedgerAsync(
        Guid accountId,
        Guid fiscalYearId,
        DateTime? fromDate,
        DateTime? toDate,
        Guid? companyId,
        Guid? branchId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// أول حركة قبل فترة معينة.
    /// تستخدم لحساب Opening Balance.
    /// </summary>
    Task<decimal> GetOpeningBalanceAsync(
        Guid accountId,
        Guid fiscalYearId,
        DateTime? fromDate,
        Guid? companyId,
        Guid? branchId,
        CancellationToken cancellationToken = default);

    IQueryable<LedgerTransaction> GetQueryable();
}