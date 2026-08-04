namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Queries.GetById;

public sealed class PurchaseReturnLineDto
{
    public Guid Id { get; init; }

    public Guid PurchaseLineId { get; init; }

    public Guid ProductId { get; init; }

    public string ProductCode { get; init; } = string.Empty;

    public string ProductName { get; init; } = string.Empty;

    public decimal Quantity { get; init; }

    public decimal UnitCost { get; init; }

    public decimal LineTotal { get; init; }

    public string? Notes { get; init; }
}