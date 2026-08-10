using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Commands.DeleteSalesDelivery;

public sealed record DeleteSalesDeliveryCommand(
    Guid Id
) : IRequest;