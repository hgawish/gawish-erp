using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(Guid id);

    Task<Warehouse?> GetByCodeAsync(string code);

    Task<(List<Warehouse> Items, int TotalCount)> GetAllAsync(
        string? search,
        bool? isActive,
        string? sortBy,
        bool descending,
        int pageNumber,
        int pageSize);

    Task<bool> ExistsAsync(Guid id);

    void Add(Warehouse warehouse);

    void Update(Warehouse warehouse);

    void Activate(Warehouse warehouse);

    void Deactivate(Warehouse warehouse);
}