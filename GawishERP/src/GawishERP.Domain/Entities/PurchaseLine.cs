using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class PurchaseLine : BaseEntity
{
    public Guid PurchaseHeaderId { get; private set; }

    public Guid ProductId { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal UnitCost { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal TaxAmount { get; private set; }

    public decimal LineTotal { get; private set; }

    public string BatchNumber { get; private set; } = string.Empty;

    public DateTime? ExpiryDate { get; private set; }

    public string? Notes { get; private set; }

    // Navigation

    public PurchaseHeader PurchaseHeader { get; private set; } = null!;

    public Product Product { get; private set; } = null!;

    private PurchaseLine()
    {
    }

    public PurchaseLine(
        Guid productId,
        decimal quantity,
        decimal unitCost,
        decimal discountAmount,
        decimal taxAmount,
        string batchNumber,
        DateTime? expiryDate,
        string? notes)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Product is required.", nameof(productId));

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(quantity));

        if (unitCost < 0)
            throw new ArgumentException(
                "Unit Cost cannot be negative.",
                nameof(unitCost));

        if (discountAmount < 0)
            throw new ArgumentException(
                "Discount cannot be negative.",
                nameof(discountAmount));

        if (taxAmount < 0)
            throw new ArgumentException(
                "Tax cannot be negative.",
                nameof(taxAmount));

        if (string.IsNullOrWhiteSpace(batchNumber))
            throw new ArgumentException(
                "Batch Number is required.",
                nameof(batchNumber));

        if (expiryDate.HasValue &&
            expiryDate.Value.Date < DateTime.UtcNow.Date)
        {
            throw new ArgumentException(
                "Expiry Date cannot be in the past.",
                nameof(expiryDate));
        }

        ProductId = productId;

        Quantity = quantity;

        UnitCost = unitCost;

        DiscountAmount = discountAmount;

        TaxAmount = taxAmount;

        BatchNumber = batchNumber.Trim().ToUpperInvariant();

        ExpiryDate = expiryDate;

        Notes = notes;

        CalculateTotal();
    }

    private void CalculateTotal()
    {
        LineTotal =
            (Quantity * UnitCost)
            - DiscountAmount
            + TaxAmount;
    }
}