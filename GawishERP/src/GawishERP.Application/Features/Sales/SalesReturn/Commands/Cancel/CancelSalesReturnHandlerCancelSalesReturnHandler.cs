using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesReturn.Commands.Cancel;

public sealed class CancelSalesReturnHandler
    : IRequestHandler<CancelSalesReturnCommand, CancelSalesReturnResponse>
{
    private readonly ISalesReturnRepository _salesReturnRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSalesReturnHandler(
        ISalesReturnRepository salesReturnRepository,
        IInventoryService inventoryService,
        IUnitOfWork unitOfWork)
    {
        _salesReturnRepository = salesReturnRepository;
        _inventoryService = inventoryService;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelSalesReturnResponse> Handle(
        CancelSalesReturnCommand request,
        CancellationToken cancellationToken)
    {
        var salesReturn =
            await _salesReturnRepository.GetByIdWithLinesAsync(
                request.SalesReturnId,
                cancellationToken);

        if (salesReturn is null)
            throw new InvalidOperationException(
                "Sales return document not found.");

        if (salesReturn.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Sales return document already cancelled.");

        if (salesReturn.Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Only posted sales return documents can be cancelled.");

        foreach (var line in salesReturn.Lines)
        {
            await _inventoryService.ReverseSalesReturnAsync(
                line.ProductId,
                salesReturn.WarehouseId,
                line.Quantity,
                line.UnitPrice,
                salesReturn.DocumentDate,
                salesReturn.Id,
                salesReturn.DocumentNumber,
                salesReturn.Notes,
                cancellationToken);
        }

        salesReturn.Cancel();

        _salesReturnRepository.Update(salesReturn);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CancelSalesReturnResponse
        {
            Id = salesReturn.Id,
            DocumentNumber = salesReturn.DocumentNumber,
            Status = salesReturn.Status.ToString(),
            Message = "Sales return cancelled successfully."
        };
    }
}