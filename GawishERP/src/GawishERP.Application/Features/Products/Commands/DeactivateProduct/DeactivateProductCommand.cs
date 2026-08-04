using MediatR;

namespace GawishERP.Application.Features.Products.Commands.DeactivateProduct;

public sealed record DeactivateProductCommand(Guid Id) : IRequest;