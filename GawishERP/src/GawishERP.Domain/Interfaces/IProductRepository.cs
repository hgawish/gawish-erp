using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id);

    Task<Product?> GetByCodeAsync(string code);

    Task<List<Product>> GetAllAsync(
        string? search,
        int pageNumber,
        int pageSize);

    void Add(Product product);

    void Update(Product product);

    void Activate(Product product);

    void Deactivate(Product product);
}