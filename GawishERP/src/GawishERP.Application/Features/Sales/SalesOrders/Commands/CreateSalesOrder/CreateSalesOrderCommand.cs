using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Commands.CreateSalesOrder;

public sealed record CreateSalesOrderCommand(
    Guid CustomerId,
    Guid? SalesQuotationId,
    DateTime DocumentDate,
    string? Notes,
    List<CreateSalesOrderLineDto> Lines)
    : IRequest<Guid>;

public sealed record CreateSalesOrderLineDto(
    Guid ProductId,
    Guid WarehouseId,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountPercent,
    decimal TaxPercent);