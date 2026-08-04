namespace GawishERP.Application.Features.Sales.SalesReturn.Queries.GetById;

public sealed class GetSalesReturnByIdResponse
{
    public Guid Id { get; set; }

    public string DocumentNumber { get; set; } = string.Empty;

    public DateTime DocumentDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public Guid SalesId { get; set; }

    public Guid CustomerId { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    public Guid WarehouseId { get; set; }

    public string WarehouseName { get; set; } = string.Empty;

    public string ReturnReason { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public string? Notes { get; set; }

    public List<SalesReturnLineDto> Lines { get; set; } = new();
}