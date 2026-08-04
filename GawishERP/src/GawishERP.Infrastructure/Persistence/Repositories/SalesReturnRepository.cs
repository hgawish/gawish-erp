using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public class SalesReturnRepository : ISalesReturnRepository
{
    private readonly ApplicationDbContext _context;

    public SalesReturnRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(SalesReturnHeader salesReturn)
    {
        _context.SalesReturnHeaders.Add(salesReturn);
    }

    public void Update(SalesReturnHeader salesReturn)
    {
        _context.SalesReturnHeaders.Update(salesReturn);
    }

    public async Task<SalesReturnHeader?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesReturnHeaders
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<SalesReturnHeader?> GetByIdWithLinesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesReturnHeaders
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .Include(x => x.Customer)
            .Include(x => x.Warehouse)
            .Include(x => x.Sales)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<SalesReturnHeader?> GetByIdForViewAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesReturnHeaders
            .Include(x => x.Customer)
            .Include(x => x.Warehouse)
            .Include(x => x.Sales)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<(List<SalesReturnHeader> Items, int TotalCount)> GetAllAsync(
        string? search,
        Guid? customerId,
        Guid? warehouseId,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.SalesReturnHeaders
            .Include(x => x.Customer)
            .Include(x => x.Warehouse)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.DocumentNumber.Contains(search) ||
                x.Customer.Name.Contains(search));
        }

        if (customerId.HasValue)
        {
            query = query.Where(x =>
                x.CustomerId == customerId.Value);
        }

        if (warehouseId.HasValue)
        {
            query = query.Where(x =>
                x.WarehouseId == warehouseId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(x =>
                x.DocumentDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(x =>
                x.DocumentDate <= toDate.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.DocumentDate)
            .ThenByDescending(x => x.DocumentNumber)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public IQueryable<SalesReturnHeader> GetQueryable()
    {
        return _context.SalesReturnHeaders
            .AsNoTracking();
    }
}