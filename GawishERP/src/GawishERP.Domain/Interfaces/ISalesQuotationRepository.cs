using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface ISalesQuotationRepository
{
    Task<SalesQuotation?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SalesQuotation?> GetByNumberAsync(
        string quotationNumber,
        CancellationToken cancellationToken = default);

    Task<List<SalesQuotation>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task AddAsync(
        SalesQuotation quotation,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        SalesQuotation quotation,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        SalesQuotation quotation,
        CancellationToken cancellationToken = default);
}