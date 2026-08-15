namespace GawishERP.Application.Features.Inventory.Valuation.Queries;

public sealed class GetInventoryValuationResponse
{
    public IReadOnlyList<InventoryValuationRowDto> Items { get; init; }
        = Array.Empty<InventoryValuationRowDto>();

    public decimal TotalQuantity { get; init; }

    public decimal TotalInventoryValue { get; init; }
}

public sealed class InventoryValuationRowDto
{
    public Guid ProductId { get; init; }

    public string ProductCode { get; init; } = string.Empty;

    public string ProductName { get; init; } = string.Empty;

    public Guid WarehouseId { get; init; }

    public string WarehouseCode { get; init; } = string.Empty;

    public string WarehouseName { get; init; } = string.Empty;

    public decimal Quantity { get; init; }

    public decimal AverageCost { get; init; }

    public decimal InventoryValue { get; init; }
}