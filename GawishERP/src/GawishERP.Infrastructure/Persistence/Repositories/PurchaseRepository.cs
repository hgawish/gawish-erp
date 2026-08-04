using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public sealed class PurchaseRepository : IPurchaseRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(PurchaseHeader purchase)
    {
        _context.PurchaseHeaders.Add(purchase);
    }

    public void Update(PurchaseHeader purchase)
    {
        _context.PurchaseHeaders.Update(purchase);
    }

    public async Task<PurchaseHeader?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseHeaders
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<PurchaseHeader?> GetByIdWithLinesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseHeaders
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<PurchaseHeader?> GetByIdForViewAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseHeaders
            .Include(x => x.Supplier)
            .Include(x => x.Warehouse)
            .Include(x => x.Lines)
                .ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    // ======================================================
    // Purchase List
    // ======================================================

    public async Task<IReadOnlyList<PurchaseHeader>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken = default)
    {
        IQueryable<PurchaseHeader> query = _context.PurchaseHeaders
            .Include(x => x.Supplier)
            .Include(x => x.Warehouse)
            .Include(x => x.Lines);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.DocumentNumber.Contains(search) ||
                x.InvoiceNumber.Contains(search) ||
                x.Supplier.Name.Contains(search));
        }

        query = (sortBy?.ToLower()) switch
        {
            "documentnumber" => descending
                ? query.OrderByDescending(x => x.DocumentNumber)
                : query.OrderBy(x => x.DocumentNumber),

            "invoicenumber" => descending
                ? query.OrderByDescending(x => x.InvoiceNumber)
                : query.OrderBy(x => x.InvoiceNumber),

            "supplier" => descending
                ? query.OrderByDescending(x => x.Supplier.Name)
                : query.OrderBy(x => x.Supplier.Name),

            "nettotal" => descending
                ? query.OrderByDescending(x => x.NetTotal)
                : query.OrderBy(x => x.NetTotal),

            _ => descending
                ? query.OrderByDescending(x => x.DocumentDate)
                : query.OrderBy(x => x.DocumentDate)
        };

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        IQueryable<PurchaseHeader> query = _context.PurchaseHeaders
            .Include(x => x.Supplier);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.DocumentNumber.Contains(search) ||
                x.InvoiceNumber.Contains(search) ||
                x.Supplier.Name.Contains(search));
        }

        return await query.CountAsync(cancellationToken);
    }
}