using GawishERP.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories.Base;

public abstract class RepositoryBase<TEntity>
    where TEntity : BaseEntity
{
    protected readonly ApplicationDbContext Context;

    protected RepositoryBase(ApplicationDbContext context)
    {
        Context = context;
    }

    protected async Task AddEntityAsync(TEntity entity)
    {
        await Context.Set<TEntity>().AddAsync(entity);
    }

    protected void UpdateEntity(TEntity entity)
    {
        Context.Set<TEntity>().Update(entity);
    }

    protected async Task<TEntity?> GetEntityByIdAsync(Guid id)
    {
        return await Context.Set<TEntity>().FindAsync(id);
    }

    protected IQueryable<TEntity> GetQueryable(bool tracking = false)
    {
        var query = Context.Set<TEntity>().AsQueryable();

        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return query;
    }

    protected async Task<int> CountAsync(
        IQueryable<TEntity> query)
    {
        return await query.CountAsync();
    }

    protected async Task<List<TEntity>> ToListAsync(
        IQueryable<TEntity> query)
    {
        return await query.ToListAsync();
    }
}