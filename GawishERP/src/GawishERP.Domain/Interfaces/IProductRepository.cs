using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);

    Task<Product?> GetByCodeAsync(string code);

    Task<(List<Product> Items, int TotalCount)> GetAllAsync(
        string? search,
        bool? isActive,
        string? sortBy,
        bool descending,
        int pageNumber,
        int pageSize);

    Task<bool> ExistsAsync(Guid id);

    void Add(Product product);

    void Update(Product product);

    void Activate(Product product);

    void Deactivate(Product product);
}