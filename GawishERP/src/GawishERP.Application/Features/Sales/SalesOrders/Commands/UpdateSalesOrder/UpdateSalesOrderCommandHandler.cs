using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Interfaces.Repositories;
using GawishERP.Domain.Common;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesOrders.Commands.UpdateSalesOrder;

public sealed class UpdateSalesOrderCommandHandler
    : IRequestHandler<UpdateSalesOrderCommand>
{
    private readonly ISalesOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSalesOrderCommandHandler(
        ISalesOrderRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpdateSalesOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (order is null)
            throw new Exception("Sales Order not found.");

        if (order.Status != DocumentStatus.Draft)
            throw new Exception(
                "Only Draft Sales Orders can be edited.");

        //
        // سيتم بعد قليل إضافة ReplaceLines()
        // داخل SalesOrder حتى يكون التعديل نظيفاً
        //

        order.SetNotes(request.Notes);

        order.ClearLines();

        foreach (var line in request.Lines)
        {
            order.AddLine(
                line.ProductId,
                line.WarehouseId,
                line.Quantity,
                line.UnitPrice,
                line.DiscountPercent,
                line.TaxPercent);
        }

        await _repository.UpdateAsync(
            order,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}