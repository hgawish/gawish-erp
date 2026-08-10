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

    public async Task<InventoryOperationResult>
        AddPurchaseReturnAsync(
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
            unitCost,
            transactionDate,
            referenceNumber,
            referenceId,
            notes,
            cancellationToken);
    }

    //=========================================================
    // Reverse Purchase
    //=========================================================

    public async Task<InventoryOperationResult>
        ReversePurchaseAsync(
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
            unitCost,
            transactionDate,
            referenceNumber,
            referenceId,
            notes,
            cancellationToken);
    }

    //=========================================================
    // Reverse Purchase Return
    //=========================================================
    //
    // Original Purchase Return:
    //
    //     Inventory -
    //
    // Reverse Purchase Return:
    //
    //     Inventory +
    //
    // The original historical cost is restored.
    //
    //=========================================================

    public async Task<InventoryOperationResult>
        ReversePurchaseReturnAsync(
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

        return new InventoryOperationResult
        {
            Quantity = quantity,
            UnitCost = unitCost,
            TotalCost = quantity * unitCost
        };
    }

    //=========================================================
    // Sale
    //=========================================================

    public async Task<InventoryOperationResult>
        AddSaleAsync(
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
            unitCost,
            transactionDate,
            referenceNumber,
            referenceId,
            notes,
            cancellationToken);
    }

    //=========================================================
    // Reverse Sale
    //=========================================================

    public async Task<InventoryOperationResult>
        ReverseSaleAsync(
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

    public async Task<InventoryOperationResult>
        AddSalesReturnAsync(
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

    public async Task<InventoryOperationResult>
        ReverseSalesReturnAsync(
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
            unitCost,
            transactionDate,
            referenceNumber,
            referenceId,
            notes,
            cancellationToken);
    }

    //=========================================================
    // Adjustment
    //=========================================================

    public async Task<InventoryOperationResult>
        AddAdjustmentAsync(
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
            unitCost,
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

        var transaction =
            new StockTransaction(
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
            balance =
                new InventoryBalance(
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
    //
    // IMPORTANT:
    //
    // The supplied unitCost is intentionally NOT used as the
    // actual cost for normal stock decreases.
    //
    // Inventory Balance AverageCost remains the source of truth
    // for normal sales and purchase returns.
    //
    // However, for a historical Purchase Return we need the
    // exact cost of the original Purchase transaction.
    //
    // Therefore:
    //
    // - unitCost > 0  => historical cost override
    // - unitCost == 0 => current AverageCost
    //
    // This allows normal existing callers to continue using
    // AverageCost while historical reversal/return operations
    // can explicitly provide their original cost.
    //
    //=========================================================

    private async Task<InventoryOperationResult>
        DecreaseStockAsync(
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
        // Determine Cost
        //=====================================================

        var actualUnitCost =
            unitCost > 0
                ? unitCost
                : balance.AverageCost;

        if (actualUnitCost < 0)
            throw new InvalidOperationException(
                "Unit cost cannot be negative.");

        var totalCost =
            quantity * actualUnitCost;

        //=====================================================
        // Decrease Inventory
        //=====================================================

        balance.Decrease(quantity);

        _balanceRepository.Update(balance);

        //=====================================================
        // Create Stock Transaction
        //=====================================================

        var transaction =
            new StockTransaction(
                productId,
                warehouseId,
                transactionType,
                quantity,
                actualUnitCost,
                referenceNumber,
                referenceId,
                transactionDate,
                notes);

        _stockRepository.Add(transaction);

        //=====================================================
        // Result
        //=====================================================

        return new InventoryOperationResult
        {
            Quantity = quantity,
            UnitCost = actualUnitCost,
            TotalCost = totalCost
        };
    }
}