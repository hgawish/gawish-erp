using System;

namespace GawishERP.Application.Common.Interfaces;

public interface IInventoryService
{
    Task AddOpeningBalanceAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        string referenceNumber,
        Guid referenceId,
        string? notes,
        CancellationToken cancellationToken = default);

    Task AddPurchaseAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default);

    Task AddPurchaseReturnAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default);

    Task ReversePurchaseAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default);

    Task AddSaleAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default);

    Task ReverseSaleAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default);

    // NEW
    Task AddSalesReturnAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default);

    // NEW
    Task ReverseSalesReturnAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default);

    Task AddAdjustmentAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        bool increase,
        DateTime transactionDate,
        string referenceNumber,
        Guid? referenceId,
        string? notes,
        CancellationToken cancellationToken = default);
}