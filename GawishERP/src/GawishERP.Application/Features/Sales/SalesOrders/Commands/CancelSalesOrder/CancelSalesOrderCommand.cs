using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Commands.CancelSalesOrder;

public sealed record CancelSalesOrderCommand(
    Guid Id)
    : IRequest<Guid>;