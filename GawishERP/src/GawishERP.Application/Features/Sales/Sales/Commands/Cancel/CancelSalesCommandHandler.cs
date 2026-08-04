using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Commands.Cancel;

public sealed class CancelSalesCommandHandler
    : IRequestHandler<CancelSalesCommand, CancelSalesResponse>
{
    private readonly ISalesRepository _salesRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSalesCommandHandler(
        ISalesRepository salesRepository,
        IInventoryService inventoryService,
        IUnitOfWork unitOfWork)
    {
        _salesRepository = salesRepository;
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelSalesResponse> Handle(
        CancelSalesCommand request,
        CancellationToken cancellationToken)
    {
        var sales =
            await _salesRepository.GetByIdWithLinesAsync(
                request.SalesId,
                cancellationToken);

        if (sales is null)
            throw new InvalidOperationException(
                "Sales document not found.");

        if (sales.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Sales document already cancelled.");

        if (sales.Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Only posted sales can be cancelled.");

        foreach (var line in sales.Lines)
        {
            await _inventoryService.ReverseSaleAsync(
                line.ProductId,
                sales.WarehouseId,
                line.Quantity,
                line.UnitPrice,
                sales.DocumentDate,
                sales.Id,
                sales.DocumentNumber,
                sales.Notes,
                cancellationToken);
        }

        sales.Cancel();

        _salesRepository.Update(sales);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelSalesResponse
        {
            Id = sales.Id,
            DocumentNumber = sales.DocumentNumber,
            Status = sales.Status.ToString(),
            Message = "Sales cancelled successfully."
        };
    }
}