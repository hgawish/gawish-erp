namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Queries.GetById;

public sealed class PurchaseReturnDetailsDto
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public DateTime DocumentDate { get; init; }

    public Guid PurchaseId { get; init; }

    public Guid SupplierId { get; init; }

    public string SupplierName { get; init; } = string.Empty;

    public Guid WarehouseId { get; init; }

    public string WarehouseName { get; init; } = string.Empty;

    public string ReturnReason { get; init; } = string.Empty;

    public decimal TotalAmount { get; init; }

    public string Status { get; init; } = string.Empty;

    public string? Notes { get; init; }

    public List<PurchaseReturnLineDto> Lines { get; init; } = new();
}