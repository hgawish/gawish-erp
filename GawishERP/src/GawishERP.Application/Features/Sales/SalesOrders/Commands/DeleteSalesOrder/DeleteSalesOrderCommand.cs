using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Commands.DeleteSalesOrder;

public sealed record DeleteSalesOrderCommand(Guid Id)
    : IRequest;