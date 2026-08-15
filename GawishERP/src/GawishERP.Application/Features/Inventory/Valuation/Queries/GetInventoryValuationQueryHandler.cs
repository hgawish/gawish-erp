using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Inventory.Valuation.Queries;

public sealed class GetInventoryValuationQueryHandler
    : IRequestHandler<GetInventoryValuationQuery, GetInventoryValuationResponse>
{
    private readonly IInventoryBalanceRepository _inventoryBalanceRepository;

    public GetInventoryValuationQueryHandler(
        IInventoryBalanceRepository inventoryBalanceRepository)
    {
        _inventoryBalanceRepository = inventoryBalanceRepository;
    }

    public async Task<GetInventoryValuationResponse> Handle(
        GetInventoryValuationQuery request,
        CancellationToken cancellationToken)
    {
        var balances = await _inventoryBalanceRepository.GetAllAsync(
            request.ProductId,
            request.WarehouseId,
            cancellationToken);

        var items = balances
            .Select(x => new InventoryValuationRowDto
            {
                ProductId = x.ProductId,
                ProductCode = x.Product.Code,
                ProductName = x.Product.Name,
                WarehouseId = x.WarehouseId,
                WarehouseCode = x.Warehouse.Code,
                WarehouseName = x.Warehouse.Name,
                Quantity = x.Quantity,
                AverageCost = x.AverageCost,
                InventoryValue = x.InventoryValue
            })
            .ToList();

        return new GetInventoryValuationResponse
        {
            Items = items,
            TotalQuantity = items.Sum(x => x.Quantity),
            TotalInventoryValue = items.Sum(x => x.InventoryValue)
        };
    }
}