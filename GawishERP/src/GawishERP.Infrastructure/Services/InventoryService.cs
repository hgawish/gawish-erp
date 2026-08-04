using GawishERP.Application.Common.Interfaces;
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

    public async Task AddPurchaseReturnAsync(
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
        await DecreaseStockAsync(
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

    public async Task ReversePurchaseAsync(
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
        await DecreaseStockAsync(
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

    public async Task AddSaleAsync(
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
        await DecreaseStockAsync(
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

    public async Task ReverseSaleAsync(
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
    }

    // ===========================
    // Sales Return
    // ===========================

    public async Task AddSalesReturnAsync(
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
    }

    public async Task ReverseSalesReturnAsync(
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
        await DecreaseStockAsync(
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
    }

    public async Task AddAdjustmentAsync(
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
        }
        else
        {
            await DecreaseStockAsync(
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
    }

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

        var balance = await _balanceRepository.GetAsync(
            productId,
            warehouseId);

        if (balance is null)
        {
            balance = new InventoryBalance(
                productId,
                warehouseId);

            balance.Increase(quantity, unitCost);

            _balanceRepository.Add(balance);
        }
        else
        {
            balance.Increase(quantity, unitCost);

            _balanceRepository.Update(balance);
        }
    }

    private async Task DecreaseStockAsync(
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
        var balance = await _balanceRepository.GetAsync(
            productId,
            warehouseId);

        if (balance is null)
            throw new InvalidOperationException(
                "Inventory balance not found.");

        balance.Decrease(quantity);

        _balanceRepository.Update(balance);

        var transaction = new StockTransaction(
            productId,
            warehouseId,
            transactionType,
            quantity,
            balance.AverageCost,
            referenceNumber,
            referenceId,
            transactionDate,
            notes);

        _stockRepository.Add(transaction);
    }
}