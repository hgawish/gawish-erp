using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Commands.DeleteSalesDelivery;

public sealed class DeleteSalesDeliveryCommandHandler
    : IRequestHandler<DeleteSalesDeliveryCommand>
{
    private readonly ISalesDeliveryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSalesDeliveryCommandHandler(
        ISalesDeliveryRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DeleteSalesDeliveryCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            throw new ArgumentException(
                "Sales Delivery ID cannot be empty.",
                nameof(request.Id));
        }

        var delivery =
            await _repository.GetByIdAsync(
                request.Id,
                cancellationToken);

        if (delivery is null)
        {
            throw new KeyNotFoundException(
                "Sales Delivery was not found.");
        }

        // لا نسمح بحذف Delivery Posted
        if (delivery.Status == Domain.Common.DocumentStatus.Posted)
        {
            throw new InvalidOperationException(
                "Posted Sales Delivery cannot be deleted.");
        }

        await _repository.DeleteAsync(
            delivery,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}