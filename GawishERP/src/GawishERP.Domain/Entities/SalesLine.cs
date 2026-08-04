using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class SalesLine : BaseEntity
{
    public Guid SalesHeaderId { get; private set; }

    public Guid ProductId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal TaxAmount { get; private set; }

    public decimal LineTotal { get; private set; }

    public string? BatchNumber { get; private set; }

    public DateTime? ExpiryDate { get; private set; }

    public string? Notes { get; private set; }

    public SalesHeader SalesHeader { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    private SalesLine()
    {
    }

    public SalesLine(
        Guid productId,
        decimal quantity,
        decimal unitPrice,
        decimal discountAmount,
        decimal taxAmount,
        string? batchNumber,
        DateTime? expiryDate,
        string? notes)
    {
        ProductId = productId;

        Quantity = quantity;

        UnitPrice = unitPrice;

        DiscountAmount = discountAmount;

        TaxAmount = taxAmount;

        BatchNumber = batchNumber;

        ExpiryDate = expiryDate;

        Notes = notes;

        LineTotal =
            (Quantity * UnitPrice)
            - DiscountAmount
            + TaxAmount;
    }
}