using GawishERP.Domain.Common;

namespace GawishERP.Application.Features.Sales.SalesOrders.Dtos;

public sealed class SalesOrderDto
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public DateTime DocumentDate { get; init; }

    public Guid CustomerId { get; init; }

    public string CustomerName { get; init; } = string.Empty;

    public Guid? SalesQuotationId { get; init; }

    public decimal TotalBeforeDiscount { get; init; }

    public decimal DiscountAmount { get; init; }

    public decimal TotalAfterDiscount { get; init; }

    public decimal TaxAmount { get; init; }

    public decimal NetAmount { get; init; }

    public DocumentStatus Status { get; init; }

    public string? Notes { get; init; }

    public List<SalesOrderLineDto> Lines { get; init; } = [];
}