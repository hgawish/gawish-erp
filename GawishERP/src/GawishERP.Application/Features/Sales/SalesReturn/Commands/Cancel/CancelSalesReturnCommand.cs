using MediatR;

namespace GawishERP.Application.Features.Sales.SalesReturn.Commands.Cancel;

public sealed record CancelSalesReturnCommand(
    Guid SalesReturnId)
    : IRequest<CancelSalesReturnResponse>;