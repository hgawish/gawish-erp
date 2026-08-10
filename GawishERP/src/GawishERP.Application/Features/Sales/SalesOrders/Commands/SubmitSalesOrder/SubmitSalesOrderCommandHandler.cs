using GawishERP.Application.Common.Interfaces.Repositories;
using GawishERP.Application.Common.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Commands.SubmitSalesOrder;

public sealed class SubmitSalesOrderCommandHandler
    : IRequestHandler<SubmitSalesOrderCommand, Guid>
{
    private readonly ISalesOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SubmitSalesOrderCommandHandler(
        ISalesOrderRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        SubmitSalesOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (order is null)
            throw new KeyNotFoundException(
                $"Sales Order '{request.Id}' was not found.");

        order.Submit();

        await _repository.UpdateAsync(
            order,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return order.Id;
    }
}