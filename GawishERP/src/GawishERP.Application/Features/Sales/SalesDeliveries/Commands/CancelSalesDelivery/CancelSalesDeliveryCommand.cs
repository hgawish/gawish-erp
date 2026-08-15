using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Commands.CancelSalesDelivery;

public sealed record CancelSalesDeliveryCommand(Guid SalesDeliveryId)
    : IRequest<CancelSalesDeliveryResponse>;