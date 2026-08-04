using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IOpeningBalanceRepository
{
    Task<OpeningBalanceHeader?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<OpeningBalanceHeader?> GetByDocumentNumberAsync(
        string documentNumber,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        string documentNumber,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<OpeningBalanceHeader>> GetPagedAsync(
        int pageNumber,
        int pageSize,
        string? search,
        string? sortBy,
        bool descending,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        string? search,
        CancellationToken cancellationToken = default);

    void Add(
        OpeningBalanceHeader document);

    void Update(
        OpeningBalanceHeader document);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}