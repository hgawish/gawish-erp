using GawishERP.Application.Common.Mapping;
using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Products.DTOs;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler
    : IRequestHandler<GetAllProductsQuery, PagedResult<ProductDto>>
{
    private readonly IProductRepository _productRepository;

    public GetAllProductsQueryHandler(
        IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductDto>> Handle(
        GetAllProductsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _productRepository.GetAllAsync(
            request.Search,
            request.IsActive,
            request.SortBy,
            request.Descending,
            request.PageNumber,
            request.PageSize);

        var items = ProductMapper.ToDtoList(result.Items);

        return new PagedResult<ProductDto>(
            items,
            result.TotalCount,
            request.PageNumber,
            request.PageSize);
    }
}