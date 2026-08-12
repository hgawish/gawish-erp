using GawishERP.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.UnitOfWork;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            var diagnostics = await ConcurrencyDiagnostics.BuildAsync(
                _context,
                cancellationToken);

            throw new InvalidOperationException(
                $"Optimistic concurrency failure during SaveChanges.\n{diagnostics}",
                ex);
        }
    }
}