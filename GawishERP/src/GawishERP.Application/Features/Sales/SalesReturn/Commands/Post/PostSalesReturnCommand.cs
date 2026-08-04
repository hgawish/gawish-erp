using MediatR;

namespace GawishERP.Application.Features.Sales.SalesReturn.Commands.Post;

public sealed record PostSalesReturnCommand(
    Guid SalesReturnId)
    : IRequest<PostSalesReturnResponse>;