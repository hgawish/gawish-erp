using GawishERP.Application.Features.Sales.SalesOrders.Dtos;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Queries.GetSalesOrders;

public sealed record GetSalesOrdersQuery()
    : IRequest<List<SalesOrderDto>>;