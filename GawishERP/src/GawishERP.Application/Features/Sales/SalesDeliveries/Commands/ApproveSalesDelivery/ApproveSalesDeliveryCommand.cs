using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Commands.ApproveSalesDelivery;

public sealed record ApproveSalesDeliveryCommand(
    Guid Id
) : IRequest<Guid>;