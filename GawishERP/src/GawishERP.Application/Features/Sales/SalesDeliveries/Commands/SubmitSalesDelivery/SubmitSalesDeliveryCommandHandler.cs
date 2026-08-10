using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Commands.SubmitSalesDelivery;

public sealed class SubmitSalesDeliveryCommandHandler
    : IRequestHandler<SubmitSalesDeliveryCommand, Guid>
{
    private readonly ISalesDeliveryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitSalesDeliveryCommandHandler(
        ISalesDeliveryRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        SubmitSalesDeliveryCommand request,
        CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            throw new ArgumentException(
                "Sales Delivery ID cannot be empty.",
                nameof(request.Id));
        }

        var delivery = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (delivery is null)
        {
            throw new InvalidOperationException(
                "Sales Delivery was not found.");
        }

        // Domain workflow
        delivery.Submit();

        await _repository.UpdateAsync(
            delivery,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return delivery.Id;
    }
}