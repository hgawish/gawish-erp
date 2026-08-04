using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IPurchaseReturnRepository
{
    void Add(PurchaseReturnHeader purchaseReturn);

    void Update(PurchaseReturnHeader purchaseReturn);

    Task<PurchaseReturnHeader?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PurchaseReturnHeader?> GetByIdWithLinesAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<PurchaseReturnHeader?> GetByIdForViewAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    IQueryable<PurchaseReturnHeader> GetQueryable();
}