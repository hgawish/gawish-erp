using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public class SupplierRepository
    : RepositoryBase<Supplier>, ISupplierRepository
{
    public SupplierRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Supplier?> GetByIdAsync(Guid id)
    {
        return await Context.Suppliers
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Supplier?> GetByCodeAsync(string code)
    {
        return await Context.Suppliers
            .Include(x => x.Account)
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<(List<Supplier> Items, int TotalCount)> GetAllAsync(
        string? search,
        bool? isActive,
        string? sortBy,
        bool descending,
        int pageNumber,
        int pageSize)
    {
        IQueryable<Supplier> query = Context.Suppliers
            .Include(x => x.Account)
            .AsQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.Code.Contains(search) ||
                x.Name.Contains(search) ||
                x.ArabicName != null && x.ArabicName.Contains(search) ||
                x.Phone != null && x.Phone.Contains(search) ||
                x.Mobile != null && x.Mobile.Contains(search) ||
                x.Email != null && x.Email.Contains(search));
        }

        // Active Filter
        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        // Sorting
        query = (sortBy?.ToLower()) switch
        {
            "code" => descending
                ? query.OrderByDescending(x => x.Code)
                : query.OrderBy(x => x.Code),

            "name" => descending
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name),

            "email" => descending
                ? query.OrderByDescending(x => x.Email)
                : query.OrderBy(x => x.Email),

            "phone" => descending
                ? query.OrderByDescending(x => x.Phone)
                : query.OrderBy(x => x.Phone),

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
        return await Context.Suppliers
            .AnyAsync(x => x.Id == id);
    }

    public void Add(Supplier supplier)
    {
        Context.Suppliers.Add(supplier);
    }

    public void Update(Supplier supplier)
    {
        UpdateEntity(supplier);
    }

    public void Activate(Supplier supplier)
    {
        supplier.Activate();
        UpdateEntity(supplier);
    }

    public void Deactivate(Supplier supplier)
    {
        supplier.Deactivate();
        UpdateEntity(supplier);
    }
}