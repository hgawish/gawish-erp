using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public class ProductRepository
    : RepositoryBase<Product>, IProductRepository
{
    public ProductRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await GetEntityByIdAsync(id);
    }

    public async Task<Product?> GetByCodeAsync(string code)
    {
        return await Context.Products
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<(List<Product> Items, int TotalCount)> GetAllAsync(
        string? search,
        bool? isActive,
        string? sortBy,
        bool descending,
        int pageNumber,
        int pageSize)
    {
        IQueryable<Product> query = GetQueryable();

        // ==========================
        // Search
        // ==========================

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.Code.Contains(search) ||
                x.Name.Contains(search) ||
                x.ArabicName != null &&
                 x.ArabicName.Contains(search));
        }

        // ==========================
        // Active Filter
        // ==========================

        if (isActive.HasValue)
        {
            query = query.Where(x =>
                x.IsActive == isActive.Value);
        }

        // ==========================
        // Sorting
        // ==========================

        query = (sortBy?.ToLower()) switch
        {
            "code" => descending
                ? query.OrderByDescending(x => x.Code)
                : query.OrderBy(x => x.Code),

            "costprice" => descending
                ? query.OrderByDescending(x => x.CostPrice)
                : query.OrderBy(x => x.CostPrice),

            "saleprice" => descending
                ? query.OrderByDescending(x => x.SalePrice)
                : query.OrderBy(x => x.SalePrice),

            _ => descending
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name)
        };

        // ==========================
        // Total Count
        // ==========================

        var totalCount = await query.CountAsync();

        // ==========================
        // Paging
        // ==========================

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await Context.Products
            .AnyAsync(x => x.Id == id);
    }

    public void Add(Product product)
    {
        Context.Products.Add(product);
    }

    public void Update(Product product)
    {
        UpdateEntity(product);
    }

    public void Activate(Product product)
    {
        product.Activate();
        UpdateEntity(product);
    }

    public void Deactivate(Product product)
    {
        product.Deactivate();
        UpdateEntity(product);
    }
}