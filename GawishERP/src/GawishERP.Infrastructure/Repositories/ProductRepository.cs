using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Persistence;
using GawishERP.Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Repositories;

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

    public async Task<List<Product>> GetAllAsync(
        string? search,
        int pageNumber,
        int pageSize)
    {
        var query = GetQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.Code.Contains(search));
        }

        return await query
            .OrderBy(x => x.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
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
        UpdateEntity(product);
    }

    public void Deactivate(Product product)
    {
        UpdateEntity(product);
    }
}