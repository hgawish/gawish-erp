using GawishERP.Application.Features.Sales.SalesDeliveries.Dtos;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Queries.GetSalesDeliveryById;

public sealed record GetSalesDeliveryByIdQuery(
    Guid Id
) : IRequest<SalesDeliveryDto>;