using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class SalesReturnLine : BaseEntity
{
    public Guid SalesReturnHeaderId { get; private set; }

    public Guid SalesLineId { get; private set; }

    public Guid ProductId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal LineTotal { get; private set; }

    public string? Notes { get; private set; }

    // Navigation

    public SalesReturnHeader SalesReturnHeader { get; private set; } = null!;

    public SalesLine SalesLine { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    private SalesReturnLine()
    {
    }

    public SalesReturnLine(
        Guid salesLineId,
        Guid productId,
        decimal quantity,
        decimal unitPrice,
        string? notes)
    {
        if (salesLineId == Guid.Empty)
            throw new ArgumentException(nameof(salesLineId));

        if (productId == Guid.Empty)
            throw new ArgumentException(nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException(nameof(quantity));

        SalesLineId = salesLineId;

        ProductId = productId;

        Quantity = quantity;

        UnitPrice = unitPrice;

        Notes = notes;

        LineTotal = Quantity * UnitPrice;
    }
}