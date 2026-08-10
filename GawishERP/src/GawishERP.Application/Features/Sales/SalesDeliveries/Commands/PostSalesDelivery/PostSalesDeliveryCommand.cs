using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Commands.PostSalesDelivery;

public sealed record PostSalesDeliveryCommand(
    Guid SalesDeliveryId
) : IRequest<Guid>;