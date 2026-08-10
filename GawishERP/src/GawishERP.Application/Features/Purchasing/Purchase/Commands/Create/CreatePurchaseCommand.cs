using MediatR;

namespace GawishERP.Application.Features.Purchasing.Purchase.Commands.Create;

public sealed record CreatePurchaseCommand
    : IRequest<CreatePurchaseResponse>
{
    //=========================================================
    // Organization
    //=========================================================

    public Guid FiscalYearId { get; init; }

    public Guid? CompanyId { get; init; }

    public Guid? BranchId { get; init; }

    //=========================================================
    // Document
    //=========================================================

    /// <summary>
    /// ERP Document Date
    /// </summary>
    public DateTime DocumentDate { get; init; }

    /// <summary>
    /// Supplier Invoice Number
    /// </summary>
    public string InvoiceNumber { get; init; } = string.Empty;

    /// <summary>
    /// Supplier Invoice Date
    /// </summary>
    public DateTime InvoiceDate { get; init; }

    public Guid SupplierId { get; init; }

    public Guid WarehouseId { get; init; }

    public string Currency { get; init; } = "EGP";

    public decimal ExchangeRate { get; init; } = 1;

    public string? Notes { get; init; }

    //=========================================================
    // Lines
    //=========================================================

    public List<CreatePurchaseLineDto> Lines { get; init; } = [];
}

public sealed record CreatePurchaseLineDto
{
    public Guid ProductId { get; init; }

    public decimal Quantity { get; init; }

    public decimal UnitCost { get; init; }

    public decimal DiscountAmount { get; init; }

    public decimal TaxAmount { get; init; }

    public string BatchNumber { get; init; } = string.Empty;

    public DateTime? ExpiryDate { get; init; }

    public string? Notes { get; init; }
}