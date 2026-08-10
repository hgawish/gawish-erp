using GawishERP.Application.Features.Sales.SalesDeliveries.Dtos;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Queries.GetSalesDeliveries;

public sealed record GetSalesDeliveriesQuery
    : IRequest<List<SalesDeliveryDto>>;