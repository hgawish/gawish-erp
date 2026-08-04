using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface ISalesRepository
{
    void Add(SalesHeader sales);

    void Update(SalesHeader sales);

    Task<SalesHeader?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SalesHeader?> GetByIdWithLinesAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SalesHeader?> GetByIdForViewAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    IQueryable<SalesHeader> GetQueryable();
}