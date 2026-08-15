using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IInventoryBalanceRepository
{
    Task<InventoryBalance?> GetAsync(
        Guid productId,
        Guid warehouseId);

    Task<IReadOnlyList<InventoryBalance>> GetAllAsync(
        Guid? productId = null,
        Guid? warehouseId = null,
        CancellationToken cancellationToken = default);

    void Add(
        InventoryBalance balance);

    void Update(
        InventoryBalance balance);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}