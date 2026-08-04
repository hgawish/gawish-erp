using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class PurchaseReturnLine : BaseEntity
{
    public Guid PurchaseReturnHeaderId { get; private set; }

    public Guid PurchaseLineId { get; private set; }

    public Guid ProductId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal UnitCost { get; private set; }

    public decimal LineTotal { get; private set; }

    public string? Notes { get; private set; }

    // Navigation

    public PurchaseReturnHeader PurchaseReturnHeader { get; private set; } = null!;

    public PurchaseLine PurchaseLine { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    private PurchaseReturnLine()
    {
    }

    public PurchaseReturnLine(
        Guid purchaseLineId,
        Guid productId,
        decimal quantity,
        decimal unitCost,
        string? notes)
    {
        if (purchaseLineId == Guid.Empty)
            throw new ArgumentException(nameof(purchaseLineId));

        if (productId == Guid.Empty)
            throw new ArgumentException(nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException(nameof(quantity));

        PurchaseLineId = purchaseLineId;

        ProductId = productId;

        Quantity = quantity;

        UnitCost = unitCost;

        Notes = notes;

        LineTotal = Quantity * UnitCost;
    }
}