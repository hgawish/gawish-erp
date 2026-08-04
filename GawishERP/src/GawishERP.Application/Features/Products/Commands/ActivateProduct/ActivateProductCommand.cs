using MediatR;

namespace GawishERP.Application.Features.Products.Commands.ActivateProduct;

public sealed record ActivateProductCommand(Guid Id) : IRequest;