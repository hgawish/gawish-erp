using GawishERP.Domain.Common;
using GawishERP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Repositories.Base;

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

    protected IQueryable<TEntity> GetQueryable()
    {
        return Context.Set<TEntity>().AsQueryable();
    }
}