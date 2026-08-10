using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Commands.UpdateSalesOrder;

public sealed record UpdateSalesOrderCommand(
    Guid Id,
    string? Notes,
    List<UpdateSalesOrderLineDto> Lines)
    : IRequest;

public sealed record UpdateSalesOrderLineDto(
    Guid ProductId,
    Guid WarehouseId,
    decimal Quantity,
    decimal UnitPrice,
    decimal DiscountPercent,
    decimal TaxPercent);