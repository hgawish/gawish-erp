using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface ISupplierRepository
{
    Task<Supplier?> GetByIdAsync(Guid id);

    Task<Supplier?> GetByCodeAsync(string code);

    Task<(List<Supplier> Items, int TotalCount)> GetAllAsync(
        string? search,
        bool? isActive,
        string? sortBy,
        bool descending,
        int pageNumber,
        int pageSize);

    Task<bool> ExistsAsync(Guid id);

    void Add(Supplier supplier);

    void Update(Supplier supplier);

    void Activate(Supplier supplier);

    void Deactivate(Supplier supplier);
}