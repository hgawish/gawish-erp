using GawishERP.Application.Common.Pagination;
using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Products.DTOs;
using MediatR;

namespace GawishERP.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQuery
    : PaginationRequest,
      IRequest<PagedResult<ProductDto>>
{
}