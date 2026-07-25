using GawishERP.Application.Common.CQRS;
using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Products.DTOs;
using GawishERP.Domain.Interfaces;

namespace GawishERP.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler
    : IQueryHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(
        IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);

        if (product is null)
        {
            return Result.Failure<ProductDto>(
                new Error(
                    "Products.NotFound",
                    "Product not found.",
                    ErrorType.NotFound));
        }

        return Result.Success(new ProductDto
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            ArabicName = product.ArabicName,
            CostPrice = product.CostPrice,
            SalePrice = product.SalePrice,
            IsActive = product.IsActive
        });
    }
}