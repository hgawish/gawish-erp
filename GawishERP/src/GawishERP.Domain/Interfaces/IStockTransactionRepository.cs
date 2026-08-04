using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IStockTransactionRepository
{
    Task<StockTransaction?> GetByIdAsync(
        Guid id);

    Task<List<StockTransaction>> GetByProductAsync(
        Guid productId);

    Task<List<StockTransaction>> GetByWarehouseAsync(
        Guid warehouseId);

    Task<List<StockTransaction>> GetByProductAndWarehouseAsync(
        Guid productId,
        Guid warehouseId);

    Task<List<StockTransaction>> GetByDateRangeAsync(
        DateTime from,
        DateTime to);

    Task<StockTransaction?> GetLastTransactionAsync(
        Guid productId,
        Guid warehouseId);

    void Add(
        StockTransaction transaction);
}