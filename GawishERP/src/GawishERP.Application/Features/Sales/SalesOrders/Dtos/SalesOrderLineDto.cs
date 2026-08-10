namespace GawishERP.Application.Features.Sales.SalesOrders.Dtos;

public sealed class SalesOrderLineDto
{
    public Guid Id { get; init; }

    public Guid ProductId { get; init; }

    public string ProductName { get; init; } = string.Empty;

    public Guid WarehouseId { get; init; }

    public string WarehouseName { get; init; } = string.Empty;

    public decimal Quantity { get; init; }

    public decimal DeliveredQuantity { get; init; }

    public decimal RemainingQuantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal DiscountPercent { get; init; }

    public decimal DiscountAmount { get; init; }

    public decimal TaxPercent { get; init; }

    public decimal TaxAmount { get; init; }

    public decimal NetAmount { get; init; }

    public bool IsCompleted { get; init; }
}