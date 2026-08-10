namespace GawishERP.Application.Common.Inventory;

public sealed class InventoryOperationResult
{
    public decimal Quantity { get; init; }

    public decimal UnitCost { get; init; }

    public decimal TotalCost { get; init; }
}