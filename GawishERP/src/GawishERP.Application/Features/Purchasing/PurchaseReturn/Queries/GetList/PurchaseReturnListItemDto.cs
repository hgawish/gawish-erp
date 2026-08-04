namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Queries.GetList;

public sealed class PurchaseReturnListItemDto
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public DateTime DocumentDate { get; init; }

    public string SupplierName { get; init; } = string.Empty;

    public string WarehouseName { get; init; } = string.Empty;

    public string ReturnReason { get; init; } = string.Empty;

    public decimal TotalAmount { get; init; }

    public string Status { get; init; } = string.Empty;
}