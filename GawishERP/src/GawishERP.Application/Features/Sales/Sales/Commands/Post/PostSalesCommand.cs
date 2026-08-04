using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Commands.Post;

public sealed record PostSalesCommand(
    Guid SalesId)
    : IRequest<PostSalesResponse>;