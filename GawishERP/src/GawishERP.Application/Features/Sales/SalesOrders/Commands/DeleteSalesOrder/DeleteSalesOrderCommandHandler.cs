using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Interfaces.Repositories;
using GawishERP.Domain.Common;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Commands.DeleteSalesOrder;

public sealed class DeleteSalesOrderCommandHandler
    : IRequestHandler<DeleteSalesOrderCommand>
{
    private readonly ISalesOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSalesOrderCommandHandler(
        ISalesOrderRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        DeleteSalesOrderCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (entity is null)
            throw new Exception("Sales Order not found.");

        if (entity.Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Only Draft Sales Orders can be deleted.");

        await _repository.DeleteAsync(
            entity,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}