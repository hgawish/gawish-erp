using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public sealed class SalesDeliveryLine : BaseEntity
{
    public Guid SalesDeliveryId { get; private set; }

    public Guid SalesOrderLineId { get; private set; }

    public Guid ProductId { get; private set; }

    public Guid WarehouseId { get; private set; }

    public decimal Quantity { get; private set; }

    //====================================================
    // Navigation
    //====================================================

    public SalesDelivery SalesDelivery { get; private set; } = null!;

    public SalesOrderLine SalesOrderLine { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    public Warehouse Warehouse { get; private set; } = null!;

    //====================================================
    // EF Constructor
    //====================================================

    private SalesDeliveryLine()
    {
    }

    //====================================================
    // Constructor
    //====================================================

    public SalesDeliveryLine(
        Guid salesDeliveryId,
        Guid salesOrderLineId,
        Guid productId,
        Guid warehouseId,
        decimal quantity)
    {
        if (salesDeliveryId == Guid.Empty)
            throw new ArgumentException(
                "Sales Delivery ID cannot be empty.",
                nameof(salesDeliveryId));

        if (salesOrderLineId == Guid.Empty)
            throw new ArgumentException(
                "Sales Order Line ID cannot be empty.",
                nameof(salesOrderLineId));

        if (productId == Guid.Empty)
            throw new ArgumentException(
                "Product ID cannot be empty.",
                nameof(productId));

        if (warehouseId == Guid.Empty)
            throw new ArgumentException(
                "Warehouse ID cannot be empty.",
                nameof(warehouseId));

        if (quantity <= 0)
            throw new ArgumentException(
                "Delivery quantity must be greater than zero.",
                nameof(quantity));

        SalesDeliveryId = salesDeliveryId;
        SalesOrderLineId = salesOrderLineId;
        ProductId = productId;
        WarehouseId = warehouseId;
        Quantity = quantity;
    }

    public void UpdateQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new InvalidOperationException(
                "Delivery quantity must be greater than zero.");

        Quantity = quantity;
    }
}