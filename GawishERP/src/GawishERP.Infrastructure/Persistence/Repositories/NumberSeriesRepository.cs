using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public sealed class NumberSeriesRepository
    : RepositoryBase<NumberSeries>, INumberSeriesRepository
{
    public NumberSeriesRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<NumberSeries?> GetByIdAsync(Guid id)
    {
        return await GetEntityByIdAsync(id);
    }

    public async Task<NumberSeries?> GetByDocumentTypeAsync(
        DocumentType documentType,
        Guid? companyId = null,
        Guid? branchId = null,
        Guid? fiscalYearId = null)
    {
        return await Context.NumberSeries
            .FirstOrDefaultAsync(x =>
                x.DocumentType == documentType &&
                x.CompanyId == companyId &&
                x.BranchId == branchId &&
                x.FiscalYearId == fiscalYearId &&
                x.IsActive);
    }

    public async Task<(List<NumberSeries> Items, int TotalCount)> GetAllAsync(
        string? search,
        bool? isActive,
        DocumentType? documentType,
        int pageNumber,
        int pageSize)
    {
        IQueryable<NumberSeries> query = GetQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            query = query.Where(x =>
                x.Prefix.Contains(search));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x =>
                x.IsActive == isActive.Value);
        }

        if (documentType.HasValue)
        {
            query = query.Where(x =>
                x.DocumentType == documentType.Value);
        }

        query = query.OrderBy(x => x.DocumentType);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await Context.NumberSeries
            .AnyAsync(x => x.Id == id);
    }

    public void Add(NumberSeries numberSeries)
    {
        Context.NumberSeries.Add(numberSeries);
    }

    public void Update(NumberSeries numberSeries)
    {
        UpdateEntity(numberSeries);
    }

    public void Activate(NumberSeries numberSeries)
    {
        numberSeries.Activate();
        UpdateEntity(numberSeries);
    }

    public void Deactivate(NumberSeries numberSeries)
    {
        numberSeries.Deactivate();
        UpdateEntity(numberSeries);
    }

    /// <summary>
    /// Generates the next document number.
    /// SaveChanges is executed by UnitOfWork.
    /// </summary>
    public async Task<string> GetNextNumberAsync(
        DocumentType documentType,
        Guid? companyId = null,
        Guid? branchId = null,
        Guid? fiscalYearId = null)
    {
        var series = await GetByDocumentTypeAsync(
            documentType,
            companyId,
            branchId,
            fiscalYearId);

        if (series is null)
        {
            throw new InvalidOperationException(
                $"Number Series not found for document type '{documentType}'.");
        }

        var nextNumber = series.GenerateNextNumber();

        UpdateEntity(series);

        return nextNumber;
    }
}