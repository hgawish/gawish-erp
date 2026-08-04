namespace GawishERP.Application.Features.Inventory.OpeningBalance.Queries.GetById;

public sealed class OpeningBalanceDetailsDto
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public Guid WarehouseId { get; init; }

    public DateTime DocumentDate { get; init; }

    public string? Notes { get; init; }

    public bool IsPosted { get; init; }

    public List<OpeningBalanceLineDetailsDto> Lines { get; init; }
        = new();
}

public sealed class OpeningBalanceLineDetailsDto
{
    public Guid ProductId { get; init; }

    public decimal Quantity { get; init; }

    public decimal UnitCost { get; init; }

    public decimal TotalCost { get; init; }

    public string? Notes { get; init; }
}