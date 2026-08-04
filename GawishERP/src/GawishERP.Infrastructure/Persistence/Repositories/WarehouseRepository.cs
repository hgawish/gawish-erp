using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public class WarehouseRepository
    : RepositoryBase<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Warehouse?> GetByIdAsync(Guid id)
    {
        return await GetEntityByIdAsync(id);
    }

    public async Task<Warehouse?> GetByCodeAsync(string code)
    {
        return await Context.Set<Warehouse>()
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<(List<Warehouse> Items, int TotalCount)> GetAllAsync(
        string? search,
        bool? isActive,
        string? sortBy,
        bool descending,
        int pageNumber,
        int pageSize)
    {
        IQueryable<Warehouse> query = GetQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.Code.Contains(search) ||
                x.Name.Contains(search) ||
                x.ArabicName != null && x.ArabicName.Contains(search) ||
                x.Manager != null && x.Manager.Contains(search) ||
                x.Phone != null && x.Phone.Contains(search));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        query = (sortBy?.ToLower()) switch
        {
            "code" => descending
                ? query.OrderByDescending(x => x.Code)
                : query.OrderBy(x => x.Code),

            "name" => descending
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name),

            "manager" => descending
                ? query.OrderByDescending(x => x.Manager)
                : query.OrderBy(x => x.Manager),

            _ => descending
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name)
        };

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await Context.Set<Warehouse>()
            .AnyAsync(x => x.Id == id);
    }

    public void Add(Warehouse warehouse)
    {
        Context.Set<Warehouse>().Add(warehouse);
    }

    public void Update(Warehouse warehouse)
    {
        UpdateEntity(warehouse);
    }

    public void Activate(Warehouse warehouse)
    {
        warehouse.Activate();
        UpdateEntity(warehouse);
    }

    public void Deactivate(Warehouse warehouse)
    {
        warehouse.Deactivate();
        UpdateEntity(warehouse);
    }
}