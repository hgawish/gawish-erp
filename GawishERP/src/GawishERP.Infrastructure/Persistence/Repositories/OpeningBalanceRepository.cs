using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public class OpeningBalanceRepository
    : RepositoryBase<OpeningBalanceHeader>,
      IOpeningBalanceRepository
{
    private DbSet<OpeningBalanceHeader> OpeningBalances =>
        Context.OpeningBalanceHeaders;

    public OpeningBalanceRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<OpeningBalanceHeader?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await OpeningBalances
            .AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<OpeningBalanceHeader?> GetByDocumentNumberAsync(
        string documentNumber,
        CancellationToken cancellationToken = default)
    {
        return await OpeningBalances
            .AsNoTracking()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(
                x => x.DocumentNumber == documentNumber,
                cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        string documentNumber,
        CancellationToken cancellationToken = default)
    {
        return await OpeningBalances
            .AsNoTracking()
            .AnyAsync(
                x => x.DocumentNumber == documentNumber,
                cancellationToken);
    }

    public async Task<IReadOnlyList<OpeningBalanceHeader>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken = default)
    {
        IQueryable<OpeningBalanceHeader> query =
            OpeningBalances
                .AsNoTracking()
                .Include(x => x.Lines);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.DocumentNumber.Contains(search));
        }

        query = (sortBy?.ToLower()) switch
        {
            "documentnumber" => descending
                ? query.OrderByDescending(x => x.DocumentNumber)
                : query.OrderBy(x => x.DocumentNumber),

            "documentdate" => descending
                ? query.OrderByDescending(x => x.DocumentDate)
                : query.OrderBy(x => x.DocumentDate),

            _ => descending
                ? query.OrderByDescending(x => x.DocumentDate)
                : query.OrderBy(x => x.DocumentDate)
        };

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountAsync(
        string? search,
        CancellationToken cancellationToken = default)
    {
        IQueryable<OpeningBalanceHeader> query =
            OpeningBalances.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.DocumentNumber.Contains(search));
        }

        return await query.CountAsync(cancellationToken);
    }

    public void Add(
        OpeningBalanceHeader document)
    {
        OpeningBalances.Add(document);
    }

    public void Update(
        OpeningBalanceHeader document)
    {
        OpeningBalances.Update(document);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }
}