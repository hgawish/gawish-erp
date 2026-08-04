using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public class CustomerRepository
    : RepositoryBase<Customer>, ICustomerRepository
{
    public CustomerRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<Customer?> GetByIdAsync(Guid id)
    {
        return await GetEntityByIdAsync(id);
    }

    public async Task<Customer?> GetByCodeAsync(string code)
    {
        return await Context.Customers
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<(List<Customer> Items, int TotalCount)> GetAllAsync(
        string? search,
        bool? isActive,
        string? sortBy,
        bool descending,
        int pageNumber,
        int pageSize)
    {
        IQueryable<Customer> query = GetQueryable();

        // Search
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.Code.Contains(search) ||
                x.Name.Contains(search) ||
                x.ArabicName != null &&
                 x.ArabicName.Contains(search) ||
                x.Phone != null &&
                 x.Phone.Contains(search) ||
                x.Email != null &&
                 x.Email.Contains(search));
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
        return await Context.Customers
            .AnyAsync(x => x.Id == id);
    }

    public void Add(Customer customer)
    {
        Context.Customers.Add(customer);
    }

    public void Update(Customer customer)
    {
        UpdateEntity(customer);
    }

    public void Activate(Customer customer)
    {
        customer.Activate();
        UpdateEntity(customer);
    }

    public void Deactivate(Customer customer)
    {
        customer.Deactivate();
        UpdateEntity(customer);
    }
}