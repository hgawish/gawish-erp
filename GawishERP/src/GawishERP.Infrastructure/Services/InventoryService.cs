using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Inventory;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;

namespace GawishERP.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly IStockTransactionRepository _stockRepository;
    private readonly IInventoryBalanceRepository _balanceRepository;

    public InventoryService(
        IStockTransactionRepository stockRepository,
        IInventoryBalanceRepository balanceRepository)
    {
        _stockRepository = stockRepository;
        _balanceRepository = balanceRepository;
    }

    public async Task AddOpeningBalanceAsync(
        Guid productId, Guid warehouseId, decimal quantity, decimal unitCost,
        DateTime transactionDate, string referenceNumber, Guid referenceId,
        string? notes, CancellationToken cancellationToken = default)
    {
        await IncreaseStockAsync(StockTransactionType.OpeningBalance, productId, warehouseId,
            quantity, unitCost, transactionDate, referenceNumber, referenceId, notes, cancellationToken);
    }

    public async Task AddPurchaseAsync(
        Guid productId, Guid warehouseId, decimal quantity, decimal unitCost,
        DateTime transactionDate, Guid referenceId, string referenceNumber,
        string? notes, CancellationToken cancellationToken = default)
    {
        await IncreaseStockAsync(StockTransactionType.Purchase, productId, warehouseId,
            quantity, unitCost, transactionDate, referenceNumber, referenceId, notes, cancellationToken);
    }

    public async Task<InventoryOperationResult> AddPurchaseReturnAsync(
        Guid productId, Guid warehouseId, decimal quantity, decimal unitCost,
        DateTime transactionDate, Guid referenceId, string referenceNumber,
        string? notes, CancellationToken cancellationToken = default)
    {
        return await DecreaseStockAsync(StockTransactionType.PurchaseReturn, productId, warehouseId,
            quantity, transactionDate, referenceNumber, referenceId, notes, unitCost, cancellationToken);
    }

    public async Task<InventoryOperationResult> ReversePurchaseAsync(
        Guid productId, Guid warehouseId, decimal quantity, decimal unitCost,
        DateTime transactionDate, Guid referenceId, string referenceNumber,
        string? notes, CancellationToken cancellationToken = default)
    {
        return await DecreaseStockAsync(StockTransactionType.PurchaseReversal, productId, warehouseId,
            quantity, transactionDate, referenceNumber, referenceId, notes, unitCost, cancellationToken);
    }

    public async Task<InventoryOperationResult> ReversePurchaseReturnAsync(
        Guid productId, Guid warehouseId, decimal quantity, decimal unitCost,
        DateTime transactionDate, Guid referenceId, string referenceNumber,
        string? notes, CancellationToken cancellationToken = default)
    {
        await IncreaseStockAsync(StockTransactionType.Purchase, productId, warehouseId,
            quantity, unitCost, transactionDate, referenceNumber, referenceId, notes, cancellationToken);

        return new InventoryOperationResult
        {
            Quantity = quantity,
            UnitCost = unitCost,
            TotalCost = quantity * unitCost
        };
    }

    public async Task<InventoryOperationResult> AddSaleAsync(
        Guid productId, Guid warehouseId, decimal quantity, decimal unitCost,
        DateTime transactionDate, Guid referenceId, string referenceNumber,
        string? notes, CancellationToken cancellationToken = default)
    {
        return await DecreaseStockAsync(StockTransactionType.Sale, productId, warehouseId,
            quantity, transactionDate, referenceNumber, referenceId, notes, null, cancellationToken);
    }

    public async Task<InventoryOperationResult> ReverseSaleAsync(
        Guid productId, Guid warehouseId, decimal quantity, decimal unitCost,
        DateTime transactionDate, Guid referenceId, string referenceNumber,
        string? notes, CancellationToken cancellationToken = default)
    {
        await IncreaseStockAsync(StockTransactionType.SalesReturn, productId, warehouseId,
            quantity, unitCost, transactionDate, referenceNumber, referenceId, notes, cancellationToken);

        return new InventoryOperationResult
        {
            Quantity = quantity,
            UnitCost = unitCost,
            TotalCost = quantity * unitCost
        };
    }

    public async Task<InventoryOperationResult> AddSalesReturnAsync(
        Guid productId, Guid warehouseId, decimal quantity, decimal unitCost,
        DateTime transactionDate, Guid referenceId, string referenceNumber,
        string? notes, CancellationToken cancellationToken = default)
    {
        await IncreaseStockAsync(StockTransactionType.SalesReturn, productId, warehouseId,
            quantity, unitCost, transactionDate, referenceNumber, referenceId, notes, cancellationToken);

        return new InventoryOperationResult
        {
            Quantity = quantity,
            UnitCost = unitCost,
            TotalCost = quantity * unitCost
        };
    }

    public async Task<InventoryOperationResult> ReverseSalesReturnAsync(
        Guid productId, Guid warehouseId, decimal quantity, decimal unitCost,
        DateTime transactionDate, Guid referenceId, string referenceNumber,
        string? notes, CancellationToken cancellationToken = default)
    {
        return await DecreaseStockAsync(StockTransactionType.Sale, productId, warehouseId,
            quantity, transactionDate, referenceNumber, referenceId, notes, unitCost, cancellationToken);
    }

    public async Task<InventoryOperationResult> AddAdjustmentAsync(
        Guid productId, Guid warehouseId, decimal quantity, decimal unitCost, bool increase,
        DateTime transactionDate, string referenceNumber, Guid? referenceId,
        string? notes, CancellationToken cancellationToken = default)
    {
        if (increase)
        {
            await IncreaseStockAsync(StockTransactionType.AdjustmentIncrease, productId, warehouseId,
                quantity, unitCost, transactionDate, referenceNumber, referenceId, notes, cancellationToken);

            return new InventoryOperationResult
            {
                Quantity = quantity,
                UnitCost = unitCost,
                TotalCost = quantity * unitCost
            };
        }

        return await DecreaseStockAsync(StockTransactionType.AdjustmentDecrease, productId, warehouseId,
            quantity, transactionDate, referenceNumber, referenceId, notes, null, cancellationToken);
    }

    private async Task IncreaseStockAsync(
        StockTransactionType transactionType, Guid productId, Guid warehouseId,
        decimal quantity, decimal unitCost, DateTime transactionDate,
        string referenceNumber, Guid? referenceId, string? notes,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than zero.");

        if (unitCost < 0)
            throw new InvalidOperationException("Unit cost cannot be negative.");

        var transaction = new StockTransaction(
            productId, warehouseId, transactionType, quantity, unitCost,
            referenceNumber, referenceId, transactionDate, notes);

        _stockRepository.Add(transaction);

        var balance = await _balanceRepository.GetAsync(productId, warehouseId);

        if (balance is null)
        {
            balance = new InventoryBalance(productId, warehouseId);
            balance.Increase(quantity, unitCost);
            _balanceRepository.Add(balance);
        }
        else
        {
            balance.Increase(quantity, unitCost);
            _balanceRepository.Update(balance);
        }
    }

    private async Task<InventoryOperationResult> DecreaseStockAsync(
        StockTransactionType transactionType, Guid productId, Guid warehouseId,
        decimal quantity, DateTime transactionDate, string referenceNumber,
        Guid? referenceId, string? notes, decimal? historicalUnitCost,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be greater than zero.");

        if (historicalUnitCost is < 0)
            throw new InvalidOperationException("Historical unit cost cannot be negative.");

        var balance = await _balanceRepository.GetAsync(productId, warehouseId);

        if (balance is null)
            throw new InvalidOperationException("Inventory balance not found.");

        if (balance.Quantity < quantity)
            throw new InvalidOperationException(
                $"Insufficient stock. Available: {balance.Quantity}, Requested: {quantity}.");

        var actualUnitCost = historicalUnitCost ?? balance.AverageCost;
        var totalCost = quantity * actualUnitCost;

        // Purchase returns and purchase reversals remove a historical purchase layer,
        // so the remaining weighted-average cost must be recalculated. Sales and other
        // decreases continue to preserve the current average cost.
        if ((transactionType == StockTransactionType.PurchaseReturn ||
             transactionType == StockTransactionType.PurchaseReversal) &&
            historicalUnitCost.HasValue)
        {
            balance.Decrease(quantity, actualUnitCost);
        }
        else
        {
            balance.Decrease(quantity);
        }

        _balanceRepository.Update(balance);

        var transaction = new StockTransaction(
            productId, warehouseId, transactionType, quantity, actualUnitCost,
            referenceNumber, referenceId, transactionDate, notes);

        _stockRepository.Add(transaction);

        return new InventoryOperationResult
        {
            Quantity = quantity,
            UnitCost = actualUnitCost,
            TotalCost = totalCost
        };
    }
}