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

    //=========================================================
    // Opening Balance
    //=========================================================

    public async Task AddOpeningBalanceAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        string referenceNumber,
        Guid referenceId,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        await IncreaseStockAsync(
            StockTransactionType.OpeningBalance,
            productId,
            warehouseId,
            quantity,
            unitCost,
            transactionDate,
            referenceNumber,
            referenceId,
            notes,
            cancellationToken);
    }

    //=========================================================
    // Purchase
    //=========================================================

    public async Task AddPurchaseAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        await IncreaseStockAsync(
            StockTransactionType.Purchase,
            productId,
            warehouseId,
            quantity,
            unitCost,
            transactionDate,
            referenceNumber,
            referenceId,
            notes,
            cancellationToken);
    }

    //=========================================================
    // Purchase Return
    //=========================================================

    public async Task<InventoryOperationResult> AddPurchaseReturnAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        return await DecreaseStockAsync(
            StockTransactionType.PurchaseReturn,
            productId,
            warehouseId,
            quantity,
            transactionDate,
            referenceNumber,
            referenceId,
            notes,
            cancellationToken);
    }

    //=========================================================
    // Reverse Purchase
    //=========================================================

    public async Task<InventoryOperationResult> ReversePurchaseAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        return await DecreaseStockAsync(
            StockTransactionType.PurchaseReturn,
            productId,
            warehouseId,
            quantity,
            transactionDate,
            referenceNumber,
            referenceId,
            notes,
            cancellationToken);
    }

    //=========================================================
    // Sale
    //=========================================================

    public async Task<InventoryOperationResult> AddSaleAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        return await DecreaseStockAsync(
            StockTransactionType.Sale,
            productId,
            warehouseId,
            quantity,
            transactionDate,
            referenceNumber,
            referenceId,
            notes,
            cancellationToken);
    }

    //=========================================================
    // Reverse Sale
    //=========================================================

    public async Task<InventoryOperationResult> ReverseSaleAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        await IncreaseStockAsync(
            StockTransactionType.SalesReturn,
            productId,
            warehouseId,
            quantity,
            unitCost,
            transactionDate,
            referenceNumber,
            referenceId,
            notes,
            cancellationToken);

        return new InventoryOperationResult
        {
            Quantity = quantity,
            UnitCost = unitCost,
            TotalCost = quantity * unitCost
        };
    }

    //=========================================================
    // Sales Return
    //=========================================================

    public async Task<InventoryOperationResult> AddSalesReturnAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        await IncreaseStockAsync(
            StockTransactionType.SalesReturn,
            productId,
            warehouseId,
            quantity,
            unitCost,
            transactionDate,
            referenceNumber,
            referenceId,
            notes,
            cancellationToken);

        return new InventoryOperationResult
        {
            Quantity = quantity,
            UnitCost = unitCost,
            TotalCost = quantity * unitCost
        };
    }

    //=========================================================
    // Reverse Sales Return
    //=========================================================

    public async Task<InventoryOperationResult> ReverseSalesReturnAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        return await DecreaseStockAsync(
            StockTransactionType.Sale,
            productId,
            warehouseId,
            quantity,
            transactionDate,
            referenceNumber,
            referenceId,
            notes,
            cancellationToken);
    }

    //=========================================================
    // Adjustment
    //=========================================================

    public async Task<InventoryOperationResult> AddAdjustmentAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        bool increase,
        DateTime transactionDate,
        string referenceNumber,
        Guid? referenceId,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        if (increase)
        {
            await IncreaseStockAsync(
                StockTransactionType.AdjustmentIncrease,
                productId,
                warehouseId,
                quantity,
                unitCost,
                transactionDate,
                referenceNumber,
                referenceId,
                notes,
                cancellationToken);

            return new InventoryOperationResult
            {
                Quantity = quantity,
                UnitCost = unitCost,
                TotalCost = quantity * unitCost
            };
        }

        return await DecreaseStockAsync(
            StockTransactionType.AdjustmentDecrease,
            productId,
            warehouseId,
            quantity,
            transactionDate,
            referenceNumber,
            referenceId,
            notes,
            cancellationToken);
    }

    //=========================================================
    // Increase Stock
    //=========================================================

    private async Task IncreaseStockAsync(
        StockTransactionType transactionType,
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        string referenceNumber,
        Guid? referenceId,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");

        if (unitCost < 0)
            throw new InvalidOperationException(
                "Unit cost cannot be negative.");

        var transaction = new StockTransaction(
            productId,
            warehouseId,
            transactionType,
            quantity,
            unitCost,
            referenceNumber,
            referenceId,
            transactionDate,
            notes);

        _stockRepository.Add(transaction);

        var balance =
            await _balanceRepository.GetAsync(
                productId,
                warehouseId);

        if (balance is null)
        {
            balance = new InventoryBalance(
                productId,
                warehouseId);

            balance.Increase(
                quantity,
                unitCost);

            _balanceRepository.Add(balance);
        }
        else
        {
            balance.Increase(
                quantity,
                unitCost);

            _balanceRepository.Update(balance);
        }
    }

    //=========================================================
    // Decrease Stock
    //=========================================================

    private async Task<InventoryOperationResult> DecreaseStockAsync(
        StockTransactionType transactionType,
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        DateTime transactionDate,
        string referenceNumber,
        Guid? referenceId,
        string? notes,
        CancellationToken cancellationToken)
    {
        if (quantity <= 0)
            throw new InvalidOperationException(
                "Quantity must be greater than zero.");

        var balance =
            await _balanceRepository.GetAsync(
                productId,
                warehouseId);

        if (balance is null)
            throw new InvalidOperationException(
                "Inventory balance not found.");

        if (balance.Quantity < quantity)
        {
            throw new InvalidOperationException(
                $"Insufficient stock. " +
                $"Available: {balance.Quantity}, " +
                $"Requested: {quantity}.");
        }

        //=====================================================
        // IMPORTANT
        // Capture AverageCost BEFORE Decrease()
        //=====================================================

        var averageCost = balance.AverageCost;

        var totalCost = quantity * averageCost;

        //=====================================================
        // Decrease Inventory Balance
        //=====================================================

        balance.Decrease(quantity);

        _balanceRepository.Update(balance);

        //=====================================================
        // Create Stock Transaction
        //=====================================================

        var transaction = new StockTransaction(
            productId,
            warehouseId,
            transactionType,
            quantity,
            averageCost,
            referenceNumber,
            referenceId,
            transactionDate,
            notes);

        _stockRepository.Add(transaction);

        //=====================================================
        // Return Actual Inventory Cost
        //=====================================================

        return new InventoryOperationResult
        {
            Quantity = quantity,
            UnitCost = averageCost,
            TotalCost = totalCost
        };
    }
}