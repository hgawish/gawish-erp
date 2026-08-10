using GawishERP.Domain.Common;

namespace GawishERP.Application.Common.Posting;

public sealed class PostingContext
{
    //=========================================================
    // Document
    //=========================================================

    public Guid DocumentId { get; init; }

    public DocumentType DocumentType { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public DateTime PostingDate { get; init; }

    public Guid FiscalYearId { get; init; }

    public Guid? CompanyId { get; init; }

    public Guid? BranchId { get; init; }

    public string? ReferenceNumber { get; init; }

    public string? Description { get; init; }

    //=========================================================
    // Amounts
    //=========================================================

    public decimal Amount { get; init; }

    public decimal TotalBeforeDiscount { get; init; }

    public decimal DiscountAmount { get; init; }

    public decimal TaxAmount { get; init; }

    public decimal CostAmount { get; init; }

    public decimal Quantity { get; init; }

    //=========================================================
    // Custom Amount
    //=========================================================

    public decimal? CustomAmount { get; init; }

    //=========================================================
    // Posting Profile
    //=========================================================

    public string? PostingProfileCode { get; init; }

    //=========================================================
    // Lines
    //=========================================================

    public IReadOnlyCollection<PostingLineContext> Lines { get; init; }
        = Array.Empty<PostingLineContext>();
}


//=============================================================
// Posting Line Context
//=============================================================

public sealed class PostingLineContext
{
    public Guid ProductId { get; init; }

    public Guid? WarehouseId { get; init; }

    public decimal Quantity { get; init; }

    //=========================================================
    // Sales Price / Purchase Cost
    //=========================================================

    public decimal UnitPrice { get; init; }

    //=========================================================
    // Actual Inventory Cost
    //
    // For Sales this should come from InventoryService / stock
    // costing, NOT from UnitPrice.
    //=========================================================

    public decimal UnitCost { get; init; }

    //=========================================================
    // Calculated Amounts
    //=========================================================

    public decimal LineAmount =>
        Quantity * UnitPrice;

    public decimal CostAmount =>
        Quantity * UnitCost;

    //=========================================================
    // Batch
    //=========================================================

    public string? BatchNumber { get; init; }

    public DateTime? ExpiryDate { get; init; }

    //=========================================================
    // Description
    //=========================================================

    public string? Description { get; init; }
}