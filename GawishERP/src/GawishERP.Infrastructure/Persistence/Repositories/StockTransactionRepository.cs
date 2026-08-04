using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public class StockTransactionRepository
    : RepositoryBase<StockTransaction>,
      IStockTransactionRepository
{
    public StockTransactionRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<StockTransaction?> GetByIdAsync(
        Guid id)
    {
        return await Context.StockTransactions
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<StockTransaction>> GetByProductAsync(
        Guid productId)
    {
        return await Context.StockTransactions
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.TransactionDate)
            .ToListAsync();
    }

    public async Task<List<StockTransaction>> GetByWarehouseAsync(
        Guid warehouseId)
    {
        return await Context.StockTransactions
            .Where(x => x.WarehouseId == warehouseId)
            .OrderBy(x => x.TransactionDate)
            .ToListAsync();
    }

    public async Task<List<StockTransaction>> GetByProductAndWarehouseAsync(
        Guid productId,
        Guid warehouseId)
    {
        return await Context.StockTransactions
            .Where(x =>
                x.ProductId == productId &&
                x.WarehouseId == warehouseId)
            .OrderBy(x => x.TransactionDate)
            .ToListAsync();
    }

    public async Task<List<StockTransaction>> GetByDateRangeAsync(
        DateTime from,
        DateTime to)
    {
        return await Context.StockTransactions
            .Where(x =>
                x.TransactionDate >= from &&
                x.TransactionDate <= to)
            .OrderBy(x => x.TransactionDate)
            .ToListAsync();
    }

    public async Task<StockTransaction?> GetLastTransactionAsync(
        Guid productId,
        Guid warehouseId)
    {
        return await Context.StockTransactions
            .Where(x =>
                x.ProductId == productId &&
                x.WarehouseId == warehouseId)
            .OrderByDescending(x => x.TransactionDate)
            .FirstOrDefaultAsync();
    }

    public void Add(
        StockTransaction transaction)
    {
        Context.StockTransactions.Add(transaction);
    }
}