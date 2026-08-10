using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Commands.ApproveSalesOrder;

public sealed record ApproveSalesOrderCommand(
    Guid Id)
    : IRequest<Guid>;