using GawishERP.Application.Features.Sales.SalesOrders.Dtos;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Queries.GetSalesOrderById;

public sealed record GetSalesOrderByIdQuery(Guid Id)
    : IRequest<SalesOrderDto>;