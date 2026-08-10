using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Commands.CreateSalesDelivery;

public sealed record CreateSalesDeliveryCommand(
    Guid SalesOrderId,
    DateTime DocumentDate,
    string? Notes,
    List<CreateSalesDeliveryLineDto> Lines
) : IRequest<Guid>;

public sealed record CreateSalesDeliveryLineDto(
    Guid SalesOrderLineId,
    decimal Quantity
);