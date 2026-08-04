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
            .FirstOrDefaultAsync(x =>
                x.ProductId == productId &&
                x.WarehouseId == warehouseId);
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