using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public class PurchaseReturnRepository
    : IPurchaseReturnRepository
{
    private readonly ApplicationDbContext _context;

    public PurchaseReturnRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    //=========================================================
    // Add
    //=========================================================

    public void Add(
        PurchaseReturnHeader purchaseReturn)
    {
        _context.PurchaseReturnHeaders.Add(
            purchaseReturn);
    }

    //=========================================================
    // Update
    //=========================================================

    public void Update(
        PurchaseReturnHeader purchaseReturn)
    {
        _context.PurchaseReturnHeaders.Update(
            purchaseReturn);
    }

    //=========================================================
    // Get By Id
    //=========================================================

    public async Task<PurchaseReturnHeader?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseReturnHeaders
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    //=========================================================
    // Get By Id With Lines
    //=========================================================

    public async Task<PurchaseReturnHeader?>
        GetByIdWithLinesAsync(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseReturnHeaders
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)

            .Include(x => x.Lines)
                .ThenInclude(x => x.PurchaseLine)

            .Include(x => x.Supplier)

            .Include(x => x.Warehouse)

            .Include(x => x.Purchase)

            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    //=========================================================
    // Get By Id For View
    //=========================================================

    public async Task<PurchaseReturnHeader?>
        GetByIdForViewAsync(
            Guid id,
            CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseReturnHeaders
            .Include(x => x.Supplier)

            .Include(x => x.Warehouse)

            .Include(x => x.Purchase)

            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)

            .Include(x => x.Lines)
                .ThenInclude(x => x.PurchaseLine)

            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    //=========================================================
    // Get Returned Quantity
    //=========================================================

    public async Task<decimal>
        GetReturnedQuantityAsync(
            Guid purchaseLineId,
            CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseReturnLines
            .Where(
                x =>
                    x.PurchaseLineId ==
                        purchaseLineId &&

                    x.PurchaseReturnHeader.Status ==
                        DocumentStatus.Posted)
            .SumAsync(
                x => x.Quantity,
                cancellationToken);
    }

    //=========================================================
    // Queryable
    //=========================================================

    public IQueryable<PurchaseReturnHeader>
        GetQueryable()
    {
        return _context.PurchaseReturnHeaders
            .AsNoTracking();
    }
}