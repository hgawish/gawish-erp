using GawishERP.Domain.Entities;

namespace GawishERP.Application.Common.Interfaces.Repositories;

public interface ISalesDeliveryRepository
{
    Task<SalesDelivery?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<SalesDelivery?> GetByDocumentNumberAsync(
        string documentNumber,
        CancellationToken cancellationToken = default);

    Task<List<SalesDelivery>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<List<SalesDelivery>> GetBySalesOrderAsync(
        Guid salesOrderId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        SalesDelivery entity,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        SalesDelivery entity,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        SalesDelivery entity,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}