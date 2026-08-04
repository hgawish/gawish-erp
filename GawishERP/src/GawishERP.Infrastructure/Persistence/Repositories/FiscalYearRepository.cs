using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public class FiscalYearRepository
    : RepositoryBase<FiscalYear>, IFiscalYearRepository
{
    public FiscalYearRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<FiscalYear?> GetByIdAsync(Guid id)
    {
        return await GetEntityByIdAsync(id);
    }

    public async Task<FiscalYear?> GetByCodeAsync(string code)
    {
        return await Context.Set<FiscalYear>()
            .FirstOrDefaultAsync(x => x.Code == code);
    }

    public async Task<FiscalYear?> GetOpenFiscalYearAsync()
    {
        return await Context.Set<FiscalYear>()
            .FirstOrDefaultAsync(x =>
                x.IsOpen &&
                !x.IsClosed &&
                x.IsActive);
    }

    public async Task<(List<FiscalYear> Items, int TotalCount)> GetAllAsync(
        string? search,
        bool? isActive,
        bool? isOpen,
        string? sortBy,
        bool descending,
        int pageNumber,
        int pageSize)
    {
        IQueryable<FiscalYear> query = GetQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.Code.Contains(search) ||
                x.Name.Contains(search));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x =>
                x.IsActive == isActive.Value);
        }

        if (isOpen.HasValue)
        {
            query = query.Where(x =>
                x.IsOpen == isOpen.Value);
        }

        query = (sortBy?.ToLower()) switch
        {
            "code" => descending
                ? query.OrderByDescending(x => x.Code)
                : query.OrderBy(x => x.Code),

            "startdate" => descending
                ? query.OrderByDescending(x => x.StartDate)
                : query.OrderBy(x => x.StartDate),

            "enddate" => descending
                ? query.OrderByDescending(x => x.EndDate)
                : query.OrderBy(x => x.EndDate),

            _ => descending
                ? query.OrderByDescending(x => x.StartDate)
                : query.OrderBy(x => x.StartDate)
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
        return await Context.Set<FiscalYear>()
            .AnyAsync(x => x.Id == id);
    }

    public void Add(FiscalYear fiscalYear)
    {
        Context.Set<FiscalYear>().Add(fiscalYear);
    }

    public void Update(FiscalYear fiscalYear)
    {
        UpdateEntity(fiscalYear);
    }

    public void Activate(FiscalYear fiscalYear)
    {
        fiscalYear.Activate();
        UpdateEntity(fiscalYear);
    }

    public void Deactivate(FiscalYear fiscalYear)
    {
        fiscalYear.Deactivate();
        UpdateEntity(fiscalYear);
    }
}