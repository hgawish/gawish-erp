using GawishERP.Domain.Entities;

namespace GawishERP.Application.Common.Interfaces.Repositories;

public interface ISalesOrderRepository
{
    Task<SalesOrder?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SalesOrder?> GetByDocumentNumberAsync(
        string documentNumber,
        CancellationToken cancellationToken = default);

    Task<List<SalesOrder>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<List<SalesOrder>> GetByCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        SalesOrder entity,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        SalesOrder entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        SalesOrder entity,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}