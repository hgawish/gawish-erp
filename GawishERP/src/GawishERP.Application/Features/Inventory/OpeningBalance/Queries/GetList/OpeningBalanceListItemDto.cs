namespace GawishERP.Application.Features.Inventory.OpeningBalance.Queries.GetList;

public sealed class OpeningBalanceListItemDto
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public DateTime DocumentDate { get; init; }

    public Guid WarehouseId { get; init; }

    public bool IsPosted { get; init; }

    public int LineCount { get; init; }

    public decimal TotalCost { get; init; }
}