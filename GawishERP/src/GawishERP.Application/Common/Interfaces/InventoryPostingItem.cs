namespace GawishERP.Application.Common.Interfaces;

public sealed class InventoryPostingItem
{
    public Guid ProductId { get; init; }

    public Guid WarehouseId { get; init; }

    public decimal Quantity { get; init; }

    public decimal UnitCost { get; init; }

    public string? ReferenceNumber { get; init; }

    public Guid? ReferenceId { get; init; }

    public DateTime TransactionDate { get; init; }

    public string? Notes { get; init; }
}