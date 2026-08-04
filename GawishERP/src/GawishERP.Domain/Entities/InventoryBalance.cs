using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class InventoryBalance : BaseEntity
{
    public Guid ProductId { get; private set; }

    public Guid WarehouseId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal AverageCost { get; private set; }

    public decimal InventoryValue => Quantity * AverageCost;

    // Navigation

    public Product Product { get; private set; } = null!;

    public Warehouse Warehouse { get; private set; } = null!;

    private InventoryBalance()
    {
    }

    public InventoryBalance(
        Guid productId,
        Guid warehouseId)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException(
                "Product is required.",
                nameof(productId));

        if (warehouseId == Guid.Empty)
            throw new ArgumentException(
                "Warehouse is required.",
                nameof(warehouseId));

        ProductId = productId;
        WarehouseId = warehouseId;

        Quantity = 0;
        AverageCost = 0;
    }

    public void Increase(
        decimal quantity,
        decimal cost)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(quantity));

        if (cost < 0)
            throw new ArgumentException(
                "Cost cannot be negative.",
                nameof(cost));

        var currentValue = Quantity * AverageCost;
        var addedValue = quantity * cost;

        Quantity += quantity;

        AverageCost =
            Quantity == 0
                ? 0
                : (currentValue + addedValue) / Quantity;
    }

    public void Decrease(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(quantity));

        if (Quantity < quantity)
            throw new InvalidOperationException(
                "Insufficient stock.");

        Quantity -= quantity;

        if (Quantity == 0)
        {
            AverageCost = 0;
        }
    }
}