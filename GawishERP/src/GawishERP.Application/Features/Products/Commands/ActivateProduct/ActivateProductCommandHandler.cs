using GawishERP.Application.Common.Exceptions;
using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Products.Commands.ActivateProduct;

public class ActivateProductCommandHandler
    : IRequestHandler<ActivateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        ActivateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);

        if (product is null)
        {
            throw new NotFoundException(
                $"Product with Id '{request.Id}' was not found.");
        }

        product.Activate();

        _productRepository.Activate(product);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}