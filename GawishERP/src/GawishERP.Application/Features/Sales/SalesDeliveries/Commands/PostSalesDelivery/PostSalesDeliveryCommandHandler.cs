using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Interfaces.Repositories;
using GawishERP.Domain.Common;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Commands.PostSalesDelivery;

public sealed class PostSalesDeliveryCommandHandler
    : IRequestHandler<PostSalesDeliveryCommand, Guid>
{
    private readonly ISalesDeliveryRepository _repository;
    private readonly IInventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;

    public PostSalesDeliveryCommandHandler(
        ISalesDeliveryRepository repository,
        IInventoryService inventoryService,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        PostSalesDeliveryCommand request,
        CancellationToken cancellationToken)
    {
        var delivery = await _repository.GetByIdAsync(
            request.SalesDeliveryId,
            cancellationToken);

        if (delivery is null)
            throw new InvalidOperationException(
                "Sales Delivery was not found.");

        if (delivery.Status != DocumentStatus.Approved)
            throw new InvalidOperationException(
                "Only approved Sales Deliveries can be posted.");

        if (!delivery.Lines.Any())
            throw new InvalidOperationException(
                "Sales Delivery has no lines.");

        foreach (var line in delivery.Lines)
        {
            await _inventoryService.AddSaleAsync(
                line.ProductId,
                line.WarehouseId,
                line.Quantity,
                0,
                delivery.DocumentDate,
                delivery.Id,
                delivery.DocumentNumber,
                delivery.Notes,
                cancellationToken);
        }

        delivery.Post();

        await _repository.UpdateAsync(
            delivery,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return delivery.Id;
    }
}