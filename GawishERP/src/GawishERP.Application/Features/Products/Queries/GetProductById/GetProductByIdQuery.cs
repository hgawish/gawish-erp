using GawishERP.Application.Common.CQRS;
using GawishERP.Application.Features.Products.DTOs;

namespace GawishERP.Application.Features.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id)
    : IQuery<ProductDto>;