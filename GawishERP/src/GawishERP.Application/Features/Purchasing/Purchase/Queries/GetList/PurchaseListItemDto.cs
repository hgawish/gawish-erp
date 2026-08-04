namespace GawishERP.Application.Features.Purchasing.Purchase.Queries.GetList;

public sealed class PurchaseListItemDto
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public DateTime DocumentDate { get; init; }

    public string InvoiceNumber { get; init; } = string.Empty;

    public DateTime InvoiceDate { get; init; }

    public Guid SupplierId { get; init; }

    public string SupplierName { get; init; } = string.Empty;

    public Guid WarehouseId { get; init; }

    public string WarehouseName { get; init; } = string.Empty;

    public decimal NetTotal { get; init; }

    public string Status { get; init; } = string.Empty;

    public int LineCount { get; init; }
}