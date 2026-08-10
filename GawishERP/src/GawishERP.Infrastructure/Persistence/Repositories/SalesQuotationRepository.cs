using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Repositories;

public sealed class SalesQuotationRepository
    : ISalesQuotationRepository
{
    private readonly ApplicationDbContext _context;

    public SalesQuotationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SalesQuotation?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesQuotations
            .Include(x => x.Customer)
            .Include(x => x.Warehouse)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<SalesQuotation?> GetByNumberAsync(
        string quotationNumber,
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesQuotations
            .Include(x => x.Customer)
            .Include(x => x.Warehouse)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(
                x => x.QuotationNumber == quotationNumber,
                cancellationToken);
    }

    public async Task<List<SalesQuotation>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SalesQuotations
            .Include(x => x.Customer)
            .Include(x => x.Warehouse)
            .OrderByDescending(x => x.QuotationDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        SalesQuotation quotation,
        CancellationToken cancellationToken = default)
    {
        await _context.SalesQuotations.AddAsync(
            quotation,
            cancellationToken);
    }

    public Task UpdateAsync(
        SalesQuotation quotation,
        CancellationToken cancellationToken = default)
    {
        _context.SalesQuotations.Update(quotation);

        return Task.CompletedTask;
    }

    public Task DeleteAsync(
        SalesQuotation quotation,
        CancellationToken cancellationToken = default)
    {
        _context.SalesQuotations.Remove(quotation);

        return Task.CompletedTask;
    }
}