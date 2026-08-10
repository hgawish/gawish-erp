using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Commands.PostSalesOrder;

public sealed record PostSalesOrderCommand(
    Guid Id)
    : IRequest<Guid>;