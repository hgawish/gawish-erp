using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Interfaces.Repositories;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Sales.SalesDeliveries.Commands.CancelSalesDelivery;

public sealed class CancelSalesDeliveryCommandHandler
    : IRequestHandler<CancelSalesDeliveryCommand, CancelSalesDeliveryResponse>
{
    private readonly ISalesDeliveryRepository _repository;
    private readonly IInventoryService _inventoryService;
    private readonly IStockTransactionRepository _stockTransactionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSalesDeliveryCommandHandler(
        ISalesDeliveryRepository repository,
        IInventoryService inventoryService,
        IStockTransactionRepository stockTransactionRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _inventoryService = inventoryService;
        _stockTransactionRepository = stockTransactionRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<CancelSalesDeliveryResponse> Handle(
        CancelSalesDeliveryCommand request,
        CancellationToken cancellationToken)
    {
        var delivery = await _repository.GetByIdAsync(
            request.SalesDeliveryId,
            cancellationToken);

        if (delivery is null)
            throw new InvalidOperationException(
                "Sales Delivery was not found.");

        if (delivery.Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Sales Delivery already cancelled.");

        if (delivery.Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Only posted Sales Deliveries can be cancelled.");

        var originalTransactions =
            await _stockTransactionRepository.GetByReferenceAsync(
                delivery.Id,
                StockTransactionType.Sale);

        foreach (var line in delivery.Lines)
        {
            var transactions = originalTransactions
                .Where(x =>
                    x.ProductId == line.ProductId &&
                    x.WarehouseId == line.WarehouseId)
                .ToList();

            var originalQuantity = transactions.Sum(x => x.Quantity);

            if (transactions.Count == 0 || originalQuantity < line.Quantity)
            {
                throw new InvalidOperationException(
                    $"Original Sales Delivery stock transaction not found or insufficient for product {line.ProductId}.");
            }

            var weightedCost = transactions.Sum(x => x.Quantity * x.UnitCost);
            var historicalUnitCost = weightedCost / originalQuantity;

            var result = await _inventoryService.ReverseSaleAsync(
                line.ProductId,
                line.WarehouseId,
                line.Quantity,
                historicalUnitCost,
                delivery.DocumentDate,
                delivery.Id,
                delivery.DocumentNumber,
                $"Reverse Sales Delivery - {delivery.DocumentNumber}",
                cancellationToken);

            if (result.Quantity != line.Quantity)
            {
                throw new InvalidOperationException(
                    "Inventory reversal quantity does not match the Sales Delivery line.");
            }
        }

        delivery.Cancel();

        await _repository.UpdateAsync(
            delivery,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return new CancelSalesDeliveryResponse
        {
            Id = delivery.Id,
            DocumentNumber = delivery.DocumentNumber,
            Status = delivery.Status.ToString(),
            Message = "Sales Delivery cancelled successfully."
        };
    }
}
