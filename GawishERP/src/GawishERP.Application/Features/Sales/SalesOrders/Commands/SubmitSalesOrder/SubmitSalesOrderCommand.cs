using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Commands.SubmitSalesOrder;

public sealed record SubmitSalesOrderCommand(
    Guid Id)
    : IRequest<Guid>;