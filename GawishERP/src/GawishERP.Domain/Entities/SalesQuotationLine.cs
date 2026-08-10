using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public sealed class SalesQuotationLine : AuditableEntity
{
    public Guid SalesQuotationId { get; private set; }

    public SalesQuotation SalesQuotation { get; private set; } = null!;

    public Guid ProductId { get; private set; }

    public Product Product { get; private set; } = null!;

    public decimal Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal DiscountPercent { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal TaxPercent { get; private set; }

    public decimal TaxAmount { get; private set; }

    public decimal LineSubTotal { get; private set; }

    public decimal LineTotal { get; private set; }

    //====================================================
    // EF Core Constructor
    //====================================================

    private SalesQuotationLine()
    {
    }

    //====================================================
    // Constructor
    //====================================================

    public SalesQuotationLine(
        Guid salesQuotationId,
        Guid productId,
        decimal quantity,
        decimal unitPrice,
        decimal discountPercent = 0,
        decimal taxPercent = 0)
    {
        if (salesQuotationId == Guid.Empty)
            throw new ArgumentException(
                "Sales quotation ID cannot be empty.",
                nameof(salesQuotationId));

        if (productId == Guid.Empty)
            throw new ArgumentException(
                "Product ID cannot be empty.",
                nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException(
                "Unit price cannot be negative.",
                nameof(unitPrice));

        if (discountPercent < 0 || discountPercent > 100)
            throw new ArgumentOutOfRangeException(
                nameof(discountPercent),
                discountPercent,
                "Discount percent must be between 0 and 100.");

        if (taxPercent < 0 || taxPercent > 100)
            throw new ArgumentOutOfRangeException(
                nameof(taxPercent),
                taxPercent,
                "Tax percent must be between 0 and 100.");

        SalesQuotationId = salesQuotationId;

        ProductId = productId;

        Quantity = quantity;

        UnitPrice = unitPrice;

        DiscountPercent = discountPercent;

        TaxPercent = taxPercent;

        Calculate();
    }

    //====================================================
    // Update
    //====================================================

    public void Update(
        decimal quantity,
        decimal unitPrice,
        decimal discountPercent,
        decimal taxPercent)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException(
                "Unit price cannot be negative.",
                nameof(unitPrice));

        if (discountPercent < 0 || discountPercent > 100)
            throw new ArgumentOutOfRangeException(
                nameof(discountPercent),
                discountPercent,
                "Discount percent must be between 0 and 100.");

        if (taxPercent < 0 || taxPercent > 100)
            throw new ArgumentOutOfRangeException(
                nameof(taxPercent),
                taxPercent,
                "Tax percent must be between 0 and 100.");

        Quantity = quantity;

        UnitPrice = unitPrice;

        DiscountPercent = discountPercent;

        TaxPercent = taxPercent;

        Calculate();
    }

    //====================================================
    // Calculation
    //====================================================

    private void Calculate()
    {
        LineSubTotal =
            Quantity * UnitPrice;

        DiscountAmount =
            LineSubTotal *
            DiscountPercent /
            100m;

        var taxableAmount =
            LineSubTotal -
            DiscountAmount;

        TaxAmount =
            taxableAmount *
            TaxPercent /
            100m;

        LineTotal =
            taxableAmount +
            TaxAmount;
    }
}