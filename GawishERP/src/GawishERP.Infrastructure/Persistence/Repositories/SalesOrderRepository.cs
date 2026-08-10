using GawishERP.Application.Common.Interfaces.Repositories;
using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public sealed class SalesOrderRepository
    : ISalesOrderRepository
{
    private readonly ApplicationDbContext _context;

    public SalesOrderRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SalesOrder?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesOrders
            .Include(x => x.Customer)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Warehouse)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<SalesOrder?> GetByDocumentNumberAsync(
        string documentNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesOrders
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Warehouse)
            .FirstOrDefaultAsync(
                x => x.DocumentNumber == documentNumber,
                cancellationToken);
    }

    public async Task<List<SalesOrder>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesOrders
            .Include(x => x.Customer)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Warehouse)
            .OrderByDescending(x => x.DocumentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<SalesOrder>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesOrders
            .Where(x => x.CustomerId == customerId)
            .Include(x => x.Customer)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Warehouse)
            .OrderByDescending(x => x.DocumentDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        SalesOrder entity,
        CancellationToken cancellationToken = default)
    {
        await _context.SalesOrders.AddAsync(
            entity,
            cancellationToken);
    }

    public Task UpdateAsync(
        SalesOrder entity,
        CancellationToken cancellationToken = default)
    {
        _context.SalesOrders.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        SalesOrder entity,
        CancellationToken cancellationToken = default)
    {
        _context.SalesOrders.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesOrders
            .AnyAsync(
                x => x.Id == id,
                cancellationToken);
    }
}