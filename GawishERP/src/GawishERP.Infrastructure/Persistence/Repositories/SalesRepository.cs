using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public class SalesRepository : ISalesRepository
{
    private readonly ApplicationDbContext _context;

    public SalesRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(SalesHeader sales)
    {
        _context.SalesHeaders.Add(sales);
    }

    public void Update(SalesHeader sales)
    {
        _context.SalesHeaders.Update(sales);
    }

    public async Task<SalesHeader?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesHeaders
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<SalesHeader?> GetByIdWithLinesAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesHeaders
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .Include(x => x.Customer)
            .Include(x => x.Warehouse)
            .AsSplitQuery()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<SalesHeader?> GetByIdForViewAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesHeaders
            .Include(x => x.Customer)
            .Include(x => x.Warehouse)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .AsSplitQuery()
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public IQueryable<SalesHeader> GetQueryable()
    {
        return _context.SalesHeaders
            .Include(x => x.Customer)
            .Include(x => x.Warehouse)
            .AsNoTracking()
            .AsSplitQuery();
    }
}