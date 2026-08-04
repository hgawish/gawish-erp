namespace GawishERP.Application.Features.Sales.Sales.Queries.GetById;

public sealed class GetSalesByIdResponse
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public DateTime DocumentDate { get; init; }

    public Guid CustomerId { get; init; }

    public string CustomerName { get; init; } = string.Empty;

    public Guid WarehouseId { get; init; }

    public string WarehouseName { get; init; } = string.Empty;

    public string Currency { get; init; } = string.Empty;

    public decimal ExchangeRate { get; init; }

    public decimal TotalBeforeDiscount { get; init; }

    public decimal DiscountAmount { get; init; }

    public decimal TaxAmount { get; init; }

    public decimal NetTotal { get; init; }

    public string Status { get; init; } = string.Empty;

    public string? Notes { get; init; }

    public List<GetSalesLineDto> Lines { get; init; } = new();
}

public sealed class GetSalesLineDto
{
    public Guid Id { get; init; }

    public Guid ProductId { get; init; }

    public string ProductCode { get; init; } = string.Empty;

    public string ProductName { get; init; } = string.Empty;

    public decimal Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal DiscountAmount { get; init; }

    public decimal TaxAmount { get; init; }

    public decimal LineTotal { get; init; }

    public string? BatchNumber { get; init; }

    public DateTime? ExpiryDate { get; init; }

    public string? Notes { get; init; }
}