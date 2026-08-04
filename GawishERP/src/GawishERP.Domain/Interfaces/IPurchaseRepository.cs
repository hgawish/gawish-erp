using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IPurchaseRepository
{
    void Add(PurchaseHeader purchase);

    void Update(PurchaseHeader purchase);

    Task<PurchaseHeader?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PurchaseHeader?> GetByIdWithLinesAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PurchaseHeader?> GetByIdForViewAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    // ===========================================
    // Purchase List
    // ===========================================

    Task<IReadOnlyList<PurchaseHeader>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        string? search,
        CancellationToken cancellationToken = default);
}