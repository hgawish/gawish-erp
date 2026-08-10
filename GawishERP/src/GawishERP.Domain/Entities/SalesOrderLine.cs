using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public sealed class SalesOrderLine : BaseEntity
{
    public Guid SalesOrderId { get; private set; }

    public Guid ProductId { get; private set; }

    public Guid WarehouseId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal DiscountPercent { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal TaxPercent { get; private set; }

    public decimal TaxAmount { get; private set; }

    public decimal LineTotalBeforeDiscount { get; private set; }

    public decimal LineTotalAfterDiscount { get; private set; }

    public decimal NetAmount { get; private set; }

    //====================================================
    // Fulfillment
    //====================================================

    public decimal DeliveredQuantity { get; private set; }

    public decimal InvoicedQuantity { get; private set; }

    public decimal RemainingQuantity =>
        Quantity - DeliveredQuantity;

    public bool IsCompleted =>
        RemainingQuantity <= 0;

    //====================================================
    // Navigation
    //====================================================

    public SalesOrder SalesOrder { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    public Warehouse Warehouse { get; private set; } = null!;

    //====================================================
    // EF Core Constructor
    //====================================================

    private SalesOrderLine()
    {
    }

    //====================================================
    // Constructor
    //====================================================

    public SalesOrderLine(
        Guid salesOrderId,
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitPrice,
        decimal discountPercent = 0,
        decimal taxPercent = 0)
    {
        if (salesOrderId == Guid.Empty)
            throw new ArgumentException(
                "Sales Order ID cannot be empty.",
                nameof(salesOrderId));

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
                "Quantity must be greater than zero.",
                nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException(
                "Unit price cannot be negative.",
                nameof(unitPrice));

        if (discountPercent < 0 || discountPercent > 100)
            throw new ArgumentException(
                "Discount percent must be between 0 and 100.",
                nameof(discountPercent));

        if (taxPercent < 0 || taxPercent > 100)
            throw new ArgumentException(
                "Tax percent must be between 0 and 100.",
                nameof(taxPercent));

        SalesOrderId = salesOrderId;

        ProductId = productId;

        WarehouseId = warehouseId;

        Quantity = quantity;

        UnitPrice = unitPrice;

        DiscountPercent = discountPercent;

        TaxPercent = taxPercent;

        DeliveredQuantity = 0;

        InvoicedQuantity = 0;

        Recalculate();
    }

    //====================================================
    // Calculations
    //====================================================

    private void Recalculate()
    {
        LineTotalBeforeDiscount =
            Quantity * UnitPrice;

        DiscountAmount =
            LineTotalBeforeDiscount *
            (DiscountPercent / 100m);

        LineTotalAfterDiscount =
            LineTotalBeforeDiscount -
            DiscountAmount;

        TaxAmount =
            LineTotalAfterDiscount *
            (TaxPercent / 100m);

        NetAmount =
            LineTotalAfterDiscount +
            TaxAmount;
    }

    //====================================================
    // Updates
    //====================================================

    public void UpdateQuantity(decimal quantity)
    {
        if (quantity <= 0)
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");

        if (DeliveredQuantity > quantity)
            throw new InvalidOperationException(
                "Quantity cannot be less than delivered quantity.");

        Quantity = quantity;

        Recalculate();
    }

    public void UpdatePrice(decimal price)
    {
        if (price < 0)
            throw new InvalidOperationException(
                "Unit price cannot be negative.");

        UnitPrice = price;

        Recalculate();
    }

    public void SetDiscount(decimal percent)
    {
        if (percent < 0 || percent > 100)
            throw new InvalidOperationException(
                "Discount percent must be between 0 and 100.");

        DiscountPercent = percent;

        Recalculate();
    }

    public void SetTax(decimal percent)
    {
        if (percent < 0 || percent > 100)
            throw new InvalidOperationException(
                "Tax percent must be between 0 and 100.");

        TaxPercent = percent;

        Recalculate();
    }

    //====================================================
    // Delivery
    //====================================================

    public void Deliver(decimal quantity)
    {
        if (quantity <= 0)
            throw new InvalidOperationException(
                "Delivered quantity must be greater than zero.");

        if (quantity > RemainingQuantity)
            throw new InvalidOperationException(
                "Delivered quantity exceeds remaining quantity.");

        DeliveredQuantity += quantity;
    }

    //====================================================
    // Invoicing
    //====================================================

    public void Invoice(decimal quantity)
    {
        if (quantity <= 0)
            throw new InvalidOperationException(
                "Invoiced quantity must be greater than zero.");

        var availableToInvoice =
            DeliveredQuantity - InvoicedQuantity;

        if (quantity > availableToInvoice)
            throw new InvalidOperationException(
                "Cannot invoice more than delivered quantity.");

        InvoicedQuantity += quantity;
    }
}