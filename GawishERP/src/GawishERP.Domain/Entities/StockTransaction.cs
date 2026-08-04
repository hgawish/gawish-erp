using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class StockTransaction : BaseEntity
{
    public Guid ProductId { get; private set; }

    public Guid WarehouseId { get; private set; }

    public StockTransactionType TransactionType { get; private set; }

    public decimal Quantity { get; private set; }

    public decimal UnitCost { get; private set; }

    public decimal TotalCost => Quantity * UnitCost;

    public string? ReferenceNumber { get; private set; }

    public Guid? ReferenceId { get; private set; }

    public DateTime TransactionDate { get; private set; }

    public string? Notes { get; private set; }

    // Navigation

    public Product Product { get; private set; } = null!;

    public Warehouse Warehouse { get; private set; } = null!;

    private StockTransaction()
    {
    }

    public StockTransaction(
        Guid productId,
        Guid warehouseId,
        StockTransactionType transactionType,
        decimal quantity,
        decimal unitCost,
        string? referenceNumber,
        Guid? referenceId,
        DateTime transactionDate,
        string? notes)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException(
                "Product is required.",
                nameof(productId));

        if (warehouseId == Guid.Empty)
            throw new ArgumentException(
                "Warehouse is required.",
                nameof(warehouseId));

        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.",
                nameof(quantity));

        if (unitCost < 0)
            throw new ArgumentException(
                "Unit Cost cannot be negative.",
                nameof(unitCost));

        if (transactionDate > DateTime.UtcNow.AddDays(1))
            throw new ArgumentException(
                "Transaction Date cannot be in the future.",
                nameof(transactionDate));

        ProductId = productId;

        WarehouseId = warehouseId;

        TransactionType = transactionType;

        Quantity = quantity;

        UnitCost = unitCost;

        ReferenceNumber = referenceNumber?.Trim().ToUpperInvariant();

        ReferenceId = referenceId;

        TransactionDate = transactionDate;

        Notes = notes?.Trim();
    }
}