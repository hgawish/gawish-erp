using MediatR;

namespace GawishERP.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? ArabicName { get; set; }

    public string? Description { get; set; }

    public decimal CostPrice { get; set; }

    public decimal SalePrice { get; set; }

    public bool IsActive { get; set; }
}