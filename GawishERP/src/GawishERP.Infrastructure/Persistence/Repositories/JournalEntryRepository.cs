using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public sealed class JournalEntryRepository : IJournalEntryRepository
{
    private readonly ApplicationDbContext _context;

    public JournalEntryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(JournalEntryHeader journalEntry) => _context.JournalEntryHeaders.Add(journalEntry);

    public void Update(JournalEntryHeader journalEntry) => _context.JournalEntryHeaders.Update(journalEntry);

    public async Task<JournalEntryHeader?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.JournalEntryHeaders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<JournalEntryHeader?> GetByIdWithLinesAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.JournalEntryHeaders
            .Include(x => x.Lines).ThenInclude(x => x.Account)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<JournalEntryHeader?> GetForReverseAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.JournalEntryHeaders
            .Include(x => x.Lines).ThenInclude(x => x.Account)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<JournalEntryHeader?> GetPostedByReferenceNumberAsync(
        string referenceNumber,
        DocumentType documentType,
        CancellationToken cancellationToken = default)
        => await _context.JournalEntryHeaders
            .Include(x => x.Lines).ThenInclude(x => x.Account)
            .FirstOrDefaultAsync(
                x => x.ReferenceNumber == referenceNumber
                    && x.DocumentType == documentType
                    && x.Status == DocumentStatus.Posted
                    && !x.IsReversed
                    && x.OriginalJournalEntryId == null,
                cancellationToken);

    public async Task<JournalEntryHeader?> GetByIdForViewAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.JournalEntryHeaders
            .Include(x => x.Lines).ThenInclude(x => x.Account)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<(List<JournalEntryHeader> Items, int TotalCount)> GetAllAsync(
        string? search, DateTime? fromDate, DateTime? toDate, DocumentStatus? status,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.JournalEntryHeaders.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.DocumentNumber.Contains(search) || x.ReferenceNumber.Contains(search));
        if (fromDate.HasValue) query = query.Where(x => x.DocumentDate >= fromDate.Value);
        if (toDate.HasValue) query = query.Where(x => x.DocumentDate <= toDate.Value);
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.DocumentDate).ThenByDescending(x => x.DocumentNumber)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public async Task<(List<JournalEntryHeader> Items, int TotalCount)> GetOpeningBalancesAsync(
        string? search, DateTime? fromDate, DateTime? toDate, DocumentStatus? status,
        int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.JournalEntryHeaders.AsNoTracking()
            .Where(x => x.DocumentType == DocumentType.OpeningBalance).AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.DocumentNumber.Contains(search) || x.ReferenceNumber.Contains(search));
        if (fromDate.HasValue) query = query.Where(x => x.DocumentDate >= fromDate.Value);
        if (toDate.HasValue) query = query.Where(x => x.DocumentDate <= toDate.Value);
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.DocumentDate).ThenByDescending(x => x.DocumentNumber)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (items, totalCount);
    }

    public async Task<bool> ExistsOpeningBalanceAsync(
        Guid fiscalYearId, Guid? companyId, Guid? branchId,
        CancellationToken cancellationToken = default)
        => await _context.JournalEntryHeaders.AsNoTracking().AnyAsync(
            x => x.DocumentType == DocumentType.OpeningBalance
                && x.FiscalYearId == fiscalYearId
                && x.CompanyId == companyId
                && x.BranchId == branchId,
            cancellationToken);

    public IQueryable<JournalEntryHeader> GetQueryable() => _context.JournalEntryHeaders.AsNoTracking();
}