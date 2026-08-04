using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IInventoryBalanceRepository
{
    Task<InventoryBalance?> GetAsync(
        Guid productId,
        Guid warehouseId);

    void Add(
        InventoryBalance balance);

    void Update(
        InventoryBalance balance);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}