namespace GawishERP.Application.Features.Sales.SalesDeliveries.Dtos;

public sealed class SalesDeliveryDto
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public DateTime DocumentDate { get; init; }

    public Guid SalesOrderId { get; init; }

    public Guid CustomerId { get; init; }

    public string CustomerName { get; init; } = string.Empty;

    public Guid FiscalYearId { get; init; }

    public int Status { get; init; }

    public string? Notes { get; init; }

    public List<SalesDeliveryLineDto> Lines { get; init; } = [];
}

public sealed class SalesDeliveryLineDto
{
    public Guid Id { get; init; }

    public Guid SalesOrderLineId { get; init; }

    public Guid ProductId { get; init; }

    public string ProductName { get; init; } = string.Empty;

    public Guid WarehouseId { get; init; }

    public string WarehouseName { get; init; } = string.Empty;

    public decimal Quantity { get; init; }
}