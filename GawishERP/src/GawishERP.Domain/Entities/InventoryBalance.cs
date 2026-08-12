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
        Decrease(quantity, null);
    }

    /// <summary>
    /// Decreases stock and, when a historical unit cost is supplied,
    /// removes that cost from the inventory value and recalculates the
    /// remaining weighted-average cost. This is required for purchase
    /// returns/reversals; ordinary sales continue to keep the current
    /// average cost unchanged.
    /// </summary>
    public void Decrease(
        decimal quantity,
        decimal? historicalUnitCost)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(quantity));

        if (historicalUnitCost is < 0)
            throw new ArgumentException(
                "Historical unit cost cannot be negative.",
                nameof(historicalUnitCost));

        if (Quantity < quantity)
            throw new InvalidOperationException(
                "Insufficient stock.");

        var currentValue = Quantity * AverageCost;

        Quantity -= quantity;

        if (Quantity == 0)
        {
            AverageCost = 0;
            return;
        }

        if (historicalUnitCost.HasValue)
        {
            var remainingValue =
                currentValue - (quantity * historicalUnitCost.Value);

            // Protect against tiny negative values caused by decimal
            // rounding when the balance is nearly exhausted.
            if (remainingValue < 0 && remainingValue > -0.01m)
                remainingValue = 0;

            if (remainingValue < 0)
                throw new InvalidOperationException(
                    "Inventory value cannot become negative during stock reversal.");

            AverageCost = remainingValue / Quantity;
        }
    }
}