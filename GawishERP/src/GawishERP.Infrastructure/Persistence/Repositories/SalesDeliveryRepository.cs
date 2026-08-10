using GawishERP.Application.Common.Interfaces.Repositories;
using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public sealed class SalesDeliveryRepository
    : ISalesDeliveryRepository
{
    private readonly ApplicationDbContext _context;

    public SalesDeliveryRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    //=========================================================
    // GET BY ID
    //=========================================================

    public async Task<SalesDelivery?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesDeliveries

            .Include(x => x.Customer)

            .Include(x => x.SalesOrder)

            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)

            .Include(x => x.Lines)
                .ThenInclude(x => x.Warehouse)

            .Include(x => x.Lines)
                .ThenInclude(x => x.SalesOrderLine)

            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    //=========================================================
    // GET BY DOCUMENT NUMBER
    //=========================================================

    public async Task<SalesDelivery?> GetByDocumentNumberAsync(
        string documentNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesDeliveries

            .Include(x => x.Customer)

            .Include(x => x.SalesOrder)

            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)

            .Include(x => x.Lines)
                .ThenInclude(x => x.Warehouse)

            .Include(x => x.Lines)
                .ThenInclude(x => x.SalesOrderLine)

            .FirstOrDefaultAsync(
                x => x.DocumentNumber == documentNumber,
                cancellationToken);
    }

    //=========================================================
    // GET ALL
    //=========================================================

    public async Task<List<SalesDelivery>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesDeliveries

            .Include(x => x.Customer)

            .Include(x => x.SalesOrder)

            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)

            .Include(x => x.Lines)
                .ThenInclude(x => x.Warehouse)

            .Include(x => x.Lines)
                .ThenInclude(x => x.SalesOrderLine)

            .OrderByDescending(x => x.DocumentDate)

            .ToListAsync(cancellationToken);
    }

    //=========================================================
    // GET BY SALES ORDER
    //=========================================================

    public async Task<List<SalesDelivery>> GetBySalesOrderAsync(
        Guid salesOrderId,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesDeliveries

            .Where(x => x.SalesOrderId == salesOrderId)

            .Include(x => x.Customer)

            .Include(x => x.SalesOrder)

            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)

            .Include(x => x.Lines)
                .ThenInclude(x => x.Warehouse)

            .Include(x => x.Lines)
                .ThenInclude(x => x.SalesOrderLine)

            .OrderByDescending(x => x.DocumentDate)

            .ToListAsync(cancellationToken);
    }

    //=========================================================
    // ADD
    //=========================================================

    public async Task AddAsync(
        SalesDelivery entity,
        CancellationToken cancellationToken = default)
    {
        await _context.SalesDeliveries.AddAsync(
            entity,
            cancellationToken);
    }

    //=========================================================
    // UPDATE
    //=========================================================

    public Task UpdateAsync(
        SalesDelivery entity,
        CancellationToken cancellationToken = default)
    {
        _context.SalesDeliveries.Update(entity);

        return Task.CompletedTask;
    }

    //=========================================================
    // DELETE
    //=========================================================

    public Task DeleteAsync(
        SalesDelivery entity,
        CancellationToken cancellationToken = default)
    {
        _context.SalesDeliveries.Remove(entity);

        return Task.CompletedTask;
    }

    //=========================================================
    // EXISTS
    //=========================================================

    public async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesDeliveries
            .AnyAsync(
                x => x.Id == id,
                cancellationToken);
    }
}