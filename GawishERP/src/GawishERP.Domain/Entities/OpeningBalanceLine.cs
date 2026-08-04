using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class OpeningBalanceLine : BaseEntity
{
    public Guid OpeningBalanceHeaderId { get; private set; }

    public Guid ProductId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal UnitCost { get; private set; }

    public string? Notes { get; private set; }

    // Navigation

    public OpeningBalanceHeader OpeningBalanceHeader { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    private OpeningBalanceLine()
    {
    }

    public OpeningBalanceLine(
        Guid openingBalanceHeaderId,
        Guid productId,
        decimal quantity,
        decimal unitCost,
        string? notes)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Product is required.", nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));

        if (unitCost < 0)
            throw new ArgumentException("Unit cost cannot be negative.", nameof(unitCost));

        OpeningBalanceHeaderId = openingBalanceHeaderId;
        ProductId = productId;
        Quantity = quantity;
        UnitCost = unitCost;
        Notes = notes?.Trim();
    }

    public decimal TotalCost => Quantity * UnitCost;
}