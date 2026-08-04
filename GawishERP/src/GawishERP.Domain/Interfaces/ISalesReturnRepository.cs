using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface ISalesReturnRepository
{
    void Add(SalesReturnHeader salesReturn);

    void Update(SalesReturnHeader salesReturn);

    Task<SalesReturnHeader?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SalesReturnHeader?> GetByIdWithLinesAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SalesReturnHeader?> GetByIdForViewAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<(List<SalesReturnHeader> Items, int TotalCount)> GetAllAsync(
        string? search,
        Guid? customerId,
        Guid? warehouseId,
        DateTime? fromDate,
        DateTime? toDate,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    IQueryable<SalesReturnHeader> GetQueryable();
}