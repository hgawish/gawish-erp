using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public sealed class LedgerTransactionRepository
    : ILedgerTransactionRepository
{
    private readonly ApplicationDbContext _context;

    public LedgerTransactionRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(LedgerTransaction transaction)
    {
        _context.LedgerTransactions.Add(transaction);
    }

    public void AddRange(IEnumerable<LedgerTransaction> transactions)
    {
        _context.LedgerTransactions.AddRange(transactions);
    }

    public async Task<bool> ExistsForJournalEntryAsync(
        Guid journalEntryHeaderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.LedgerTransactions
            .AnyAsync(
                x => x.JournalEntryHeaderId == journalEntryHeaderId,
                cancellationToken);
    }

    public async Task<List<LedgerTransaction>> GetByJournalEntryAsync(
        Guid journalEntryHeaderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.LedgerTransactions
            .Where(x => x.JournalEntryHeaderId == journalEntryHeaderId)
            .OrderBy(x => x.PostingDate)
            .ThenBy(x => x.Id)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<List<LedgerTransaction>> GetAccountLedgerAsync(
        Guid accountId,
        Guid fiscalYearId,
        DateTime? fromDate,
        DateTime? toDate,
        Guid? companyId,
        Guid? branchId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.LedgerTransactions
            .Include(x => x.Account)
            .Where(x =>
                x.AccountId == accountId &&
                x.FiscalYearId == fiscalYearId);

        if (companyId.HasValue)
        {
            query = query.Where(x =>
                x.CompanyId == companyId.Value);
        }

        if (branchId.HasValue)
        {
            query = query.Where(x =>
                x.BranchId == branchId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(x =>
                x.PostingDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x =>
                x.PostingDate <= toDate.Value);
        }

        return await query
            .OrderBy(x => x.PostingDate)
            .ThenBy(x => x.DocumentNumber)
            .ThenBy(x => x.Id)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetOpeningBalanceAsync(
        Guid accountId,
        Guid fiscalYearId,
        DateTime? fromDate,
        Guid? companyId,
        Guid? branchId,
        CancellationToken cancellationToken = default)
    {
        if (!fromDate.HasValue)
            return 0m;

        var query = _context.LedgerTransactions
            .Where(x =>
                x.AccountId == accountId &&
                x.FiscalYearId == fiscalYearId &&
                x.PostingDate < fromDate.Value);

        if (companyId.HasValue)
        {
            query = query.Where(x =>
                x.CompanyId == companyId.Value);
        }

        if (branchId.HasValue)
        {
            query = query.Where(x =>
                x.BranchId == branchId.Value);
        }

        var debit = await query.SumAsync(
            x => x.Debit,
            cancellationToken);

        var credit = await query.SumAsync(
            x => x.Credit,
            cancellationToken);

        return debit - credit;
    }

    public IQueryable<LedgerTransaction> GetQueryable()
    {
        return _context.LedgerTransactions
            .AsNoTracking();
    }
}