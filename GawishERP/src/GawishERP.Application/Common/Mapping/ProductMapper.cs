using GawishERP.Application.Features.Products.DTOs;
using GawishERP.Domain.Entities;

namespace GawishERP.Application.Common.Mapping;

public static class ProductMapper
{
    public static ProductDto ToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            ArabicName = product.ArabicName,
            CostPrice = product.CostPrice,
            SalePrice = product.SalePrice,
            IsActive = product.IsActive
        };
    }

    public static List<ProductDto> ToDtoList(IEnumerable<Product> products)
    {
        return products
            .Select(ToDto)
            .ToList();
    }
}