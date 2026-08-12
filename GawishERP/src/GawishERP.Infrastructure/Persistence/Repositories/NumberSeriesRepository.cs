using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Data.Common;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public sealed class NumberSeriesRepository
    : RepositoryBase<NumberSeries>, INumberSeriesRepository
{
    public NumberSeriesRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<NumberSeries?> GetByIdAsync(Guid id)
        => await GetEntityByIdAsync(id);

    public async Task<NumberSeries?> GetByDocumentTypeAsync(
        DocumentType documentType,
        Guid? companyId = null,
        Guid? branchId = null,
        Guid? fiscalYearId = null)
    {
        var specificSeries = await Context.NumberSeries
            .FirstOrDefaultAsync(x =>
                x.DocumentType == documentType &&
                x.CompanyId == companyId &&
                x.BranchId == branchId &&
                x.FiscalYearId == fiscalYearId &&
                x.IsActive);

        if (specificSeries is not null)
            return specificSeries;

        return await Context.NumberSeries
            .FirstOrDefaultAsync(x =>
                x.DocumentType == documentType &&
                x.CompanyId == null &&
                x.BranchId == null &&
                x.FiscalYearId == null &&
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
            query = query.Where(x => x.Prefix.Contains(search));
        }

        if (isActive.HasValue)
            query = query.Where(x => x.IsActive == isActive.Value);

        if (documentType.HasValue)
            query = query.Where(x => x.DocumentType == documentType.Value);

        query = query.OrderBy(x => x.DocumentType);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<bool> ExistsAsync(Guid id)
        => await Context.NumberSeries.AnyAsync(x => x.Id == id);

    /// <summary>
    /// Atomically increments CurrentNumber and returns the new value.
    /// This deliberately bypasses EF change tracking for NumberSeries so
    /// RowVersion is not part of the surrounding UnitOfWork concurrency check.
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

        var connection = Context.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        try
        {
            if (shouldClose)
                await connection.OpenAsync();

            await using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE dbo.NumberSeries
                SET CurrentNumber = CurrentNumber + 1
                OUTPUT INSERTED.CurrentNumber
                WHERE Id = @id
                  AND IsActive = 1;
                """;

            var idParameter = command.CreateParameter();
            idParameter.ParameterName = "@id";
            idParameter.DbType = DbType.Guid;
            idParameter.Value = series.Id;
            command.Parameters.Add(idParameter);

            var currentTransaction = Context.Database.CurrentTransaction;
            if (currentTransaction is not null)
                command.Transaction = currentTransaction.GetDbTransaction();

            var result = await command.ExecuteScalarAsync();

            if (result is null || result == DBNull.Value)
            {
                throw new InvalidOperationException(
                    $"Number Series for '{documentType}' could not be incremented.");
            }

            var currentNumber = Convert.ToInt32(result);
            var padding = series.Padding;

            return $"{series.Prefix}{currentNumber.ToString().PadLeft(padding, '0')}";
        }
        finally
        {
            if (shouldClose)
                await connection.CloseAsync();
        }
    }

    public void Add(NumberSeries numberSeries)
        => Context.NumberSeries.Add(numberSeries);

    public void Update(NumberSeries numberSeries)
        => UpdateEntity(numberSeries);

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
}