using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Commands.SubmitSalesDelivery;

public sealed record SubmitSalesDeliveryCommand(
    Guid Id
) : IRequest<Guid>;