using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class Product : ActivatableEntity
{
    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? ArabicName { get; private set; }

    public string? Description { get; private set; }

    public decimal CostPrice { get; private set; }

    public decimal SalePrice { get; private set; }

    private Product()
    {
    }

    public Product(
        string code,
        string name,
        string? arabicName,
        string? description,
        decimal costPrice,
        decimal salePrice)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Product code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.", nameof(name));

        if (costPrice < 0)
            throw new ArgumentException("Cost price cannot be negative.", nameof(costPrice));

        if (salePrice < 0)
            throw new ArgumentException("Sale price cannot be negative.", nameof(salePrice));

        Code = code.Trim();
        Name = name.Trim();
        ArabicName = arabicName?.Trim();
        Description = description?.Trim();
        CostPrice = costPrice;
        SalePrice = salePrice;
    }

    public void Update(
        string name,
        string? arabicName,
        string? description,
        decimal costPrice,
        decimal salePrice)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name is required.", nameof(name));

        if (costPrice < 0)
            throw new ArgumentException("Cost price cannot be negative.", nameof(costPrice));

        if (salePrice < 0)
            throw new ArgumentException("Sale price cannot be negative.", nameof(salePrice));

        Name = name.Trim();
        ArabicName = arabicName?.Trim();
        Description = description?.Trim();
        CostPrice = costPrice;
        SalePrice = salePrice;
    }
}