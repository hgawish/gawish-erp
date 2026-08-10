using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IStockTransactionRepository
{
    //=========================================================
    // Get By Id
    //=========================================================

    Task<StockTransaction?> GetByIdAsync(
        Guid id);

    //=========================================================
    // Get By Product
    //=========================================================

    Task<List<StockTransaction>> GetByProductAsync(
        Guid productId);

    //=========================================================
    // Get By Warehouse
    //=========================================================

    Task<List<StockTransaction>> GetByWarehouseAsync(
        Guid warehouseId);

    //=========================================================
    // Get By Product And Warehouse
    //=========================================================

    Task<List<StockTransaction>> GetByProductAndWarehouseAsync(
        Guid productId,
        Guid warehouseId);

    //=========================================================
    // Get By Date Range
    //=========================================================

    Task<List<StockTransaction>> GetByDateRangeAsync(
        DateTime from,
        DateTime to);

    //=========================================================
    // Get Last Transaction
    //=========================================================

    Task<StockTransaction?> GetLastTransactionAsync(
        Guid productId,
        Guid warehouseId);

    //=========================================================
    // Get By Reference
    //=========================================================
    //
    // Used for historical inventory costing and document
    // reversal.
    //
    // Example:
    //
    // Sales Invoice
    // ReferenceId = SalesHeader.Id
    // TransactionType = Sale
    //
    // Sales Return
    // ReferenceId = SalesReturnHeader.Id
    // TransactionType = SalesReturn
    //
    //=========================================================

    Task<List<StockTransaction>> GetByReferenceAsync(
        Guid referenceId,
        StockTransactionType transactionType);

    //=========================================================
    // Add
    //=========================================================

    void Add(
        StockTransaction transaction);
}