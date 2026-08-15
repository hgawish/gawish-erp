using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public class InventoryBalanceRepository
    : RepositoryBase<InventoryBalance>,
      IInventoryBalanceRepository
{
    public InventoryBalanceRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<InventoryBalance?> GetAsync(
        Guid productId,
        Guid warehouseId)
    {
        return await Context.InventoryBalances
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .FirstOrDefaultAsync(x =>
                x.ProductId == productId &&
                x.WarehouseId == warehouseId);
    }

    public async Task<IReadOnlyList<InventoryBalance>> GetAllAsync(
        Guid? productId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default)
    {
        var query = Context.InventoryBalances
            .AsNoTracking()
            .Include(x => x.Product)
            .Include(x => x.Warehouse)
            .AsQueryable();

        if (productId.HasValue)
            query = query.Where(x => x.ProductId == productId.Value);

        if (warehouseId.HasValue)
            query = query.Where(x => x.WarehouseId == warehouseId.Value);

        return await query
            .OrderBy(x => x.Product.Code)
            .ThenBy(x => x.Warehouse.Code)
            .ToListAsync(cancellationToken);
    }

    public void Add(
        InventoryBalance balance)
    {
        Context.InventoryBalances.Add(balance);
    }

    public void Update(
        InventoryBalance balance)
    {
        Context.InventoryBalances.Update(balance);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }
}