using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Interfaces.Repositories;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Commands.ApproveSalesOrder;

public sealed class ApproveSalesOrderCommandHandler
    : IRequestHandler<ApproveSalesOrderCommand, Guid>
{
    private readonly ISalesOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveSalesOrderCommandHandler(
        ISalesOrderRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        ApproveSalesOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (order is null)
            throw new KeyNotFoundException(
                $"Sales Order '{request.Id}' was not found.");

        order.Approve();

        await _repository.UpdateAsync(
            order,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return order.Id;
    }
}