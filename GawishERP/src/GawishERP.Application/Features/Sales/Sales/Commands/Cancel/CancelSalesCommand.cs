using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Commands.Cancel;

public sealed record CancelSalesCommand(
    Guid SalesId)
    : IRequest<CancelSalesResponse>;