using GawishERP.Application.Common.Inventory;

namespace GawishERP.Application.Common.Interfaces;

public interface IInventoryService
{
    //=========================================================
    // Opening Balance
    //=========================================================

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

    //=========================================================
    // Purchase
    //=========================================================

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

    //=========================================================
    // Purchase Return
    //=========================================================

    Task<InventoryOperationResult> AddPurchaseReturnAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default);

    //=========================================================
    // Reverse Purchase
    //=========================================================

    Task<InventoryOperationResult> ReversePurchaseAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default);

    //=========================================================
    // Reverse Purchase Return
    //=========================================================

    Task<InventoryOperationResult> ReversePurchaseReturnAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default);

    //=========================================================
    // Sale
    //=========================================================

    Task<InventoryOperationResult> AddSaleAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default);

    //=========================================================
    // Reverse Sale
    //=========================================================

    Task<InventoryOperationResult> ReverseSaleAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default);

    //=========================================================
    // Sales Return
    //=========================================================

    Task<InventoryOperationResult> AddSalesReturnAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default);

    //=========================================================
    // Reverse Sales Return
    //=========================================================

    Task<InventoryOperationResult> ReverseSalesReturnAsync(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitCost,
        DateTime transactionDate,
        Guid referenceId,
        string referenceNumber,
        string? notes,
        CancellationToken cancellationToken = default);

    //=========================================================
    // Adjustment
    //=========================================================

    Task<InventoryOperationResult> AddAdjustmentAsync(
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