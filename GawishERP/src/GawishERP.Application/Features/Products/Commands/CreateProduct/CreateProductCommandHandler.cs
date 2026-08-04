using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var existingProduct =
            await _productRepository.GetByCodeAsync(request.Code);

        if (existingProduct is not null)
        {
            throw new InvalidOperationException(
                $"Product code '{request.Code}' already exists.");
        }

        var product = new Product(
            request.Code,
            request.Name,
            request.ArabicName,
            request.Description,
            request.CostPrice,
            request.SalePrice);

        _productRepository.Add(product);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}