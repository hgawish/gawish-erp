using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public sealed class FinancialStatementNodeRepository
    : IFinancialStatementNodeRepository
{
    private readonly ApplicationDbContext _context;

    public FinancialStatementNodeRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(FinancialStatementNode entity)
    {
        _context.FinancialStatementNodes.Add(entity);
    }

    public void Update(FinancialStatementNode entity)
    {
        _context.FinancialStatementNodes.Update(entity);
    }

    public void Remove(FinancialStatementNode entity)
    {
        _context.FinancialStatementNodes.Remove(entity);
    }

    public async Task<FinancialStatementNode?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.FinancialStatementNodes
            .Include(x => x.Parent)
            .Include(x => x.Children)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<FinancialStatementNode?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _context.FinancialStatementNodes
            .FirstOrDefaultAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task<bool> ExistsByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        return await _context.FinancialStatementNodes
            .AnyAsync(
                x => x.Code == code,
                cancellationToken);
    }

    public async Task<List<FinancialStatementNode>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.FinancialStatementNodes
            .Include(x => x.Parent)
            .Include(x => x.Children)
            .OrderBy(x => x.StatementType)
            .ThenBy(x => x.SortOrder)
            .ThenBy(x => x.Code)
            .ToListAsync(cancellationToken);
    }

    public IQueryable<FinancialStatementNode> GetQueryable()
    {
        return _context.FinancialStatementNodes
            .Include(x => x.Parent)
            .Include(x => x.Children)
            .AsNoTracking();
    }
}