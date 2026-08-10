using GawishERP.Domain.Common;
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

    //=========================================================
    // Get By Id
    //=========================================================

    public async Task<StockTransaction?> GetByIdAsync(
        Guid id)
    {
        return await Context.StockTransactions
            .FirstOrDefaultAsync(
                x => x.Id == id);
    }

    //=========================================================
    // Get By Product
    //=========================================================

    public async Task<List<StockTransaction>> GetByProductAsync(
        Guid productId)
    {
        return await Context.StockTransactions
            .Where(x =>
                x.ProductId == productId)
            .OrderBy(x =>
                x.TransactionDate)
            .ToListAsync();
    }

    //=========================================================
    // Get By Warehouse
    //=========================================================

    public async Task<List<StockTransaction>> GetByWarehouseAsync(
        Guid warehouseId)
    {
        return await Context.StockTransactions
            .Where(x =>
                x.WarehouseId == warehouseId)
            .OrderBy(x =>
                x.TransactionDate)
            .ToListAsync();
    }

    //=========================================================
    // Get By Product And Warehouse
    //=========================================================

    public async Task<List<StockTransaction>>
        GetByProductAndWarehouseAsync(
            Guid productId,
            Guid warehouseId)
    {
        return await Context.StockTransactions
            .Where(x =>
                x.ProductId == productId &&
                x.WarehouseId == warehouseId)
            .OrderBy(x =>
                x.TransactionDate)
            .ToListAsync();
    }

    //=========================================================
    // Get By Date Range
    //=========================================================

    public async Task<List<StockTransaction>>
        GetByDateRangeAsync(
            DateTime from,
            DateTime to)
    {
        return await Context.StockTransactions
            .Where(x =>
                x.TransactionDate >= from &&
                x.TransactionDate <= to)
            .OrderBy(x =>
                x.TransactionDate)
            .ToListAsync();
    }

    //=========================================================
    // Get Last Transaction
    //=========================================================

    public async Task<StockTransaction?>
        GetLastTransactionAsync(
            Guid productId,
            Guid warehouseId)
    {
        return await Context.StockTransactions
            .Where(x =>
                x.ProductId == productId &&
                x.WarehouseId == warehouseId)
            .OrderByDescending(x =>
                x.TransactionDate)
            .FirstOrDefaultAsync();
    }

    //=========================================================
    // Get By Reference
    //=========================================================

    public async Task<List<StockTransaction>>
        GetByReferenceAsync(
            Guid referenceId,
            StockTransactionType transactionType)
    {
        if (referenceId == Guid.Empty)
            return new List<StockTransaction>();

        return await Context.StockTransactions
            .Where(x =>
                x.ReferenceId == referenceId &&
                x.TransactionType == transactionType)
            .OrderBy(x =>
                x.TransactionDate)
            .ThenBy(x =>
                x.Id)
            .ToListAsync();
    }

    //=========================================================
    // Add
    //=========================================================

    public void Add(
        StockTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        Context.StockTransactions.Add(
            transaction);
    }
}