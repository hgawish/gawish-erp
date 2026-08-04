using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);

    Task<Customer?> GetByCodeAsync(string code);

    Task<(List<Customer> Items, int TotalCount)> GetAllAsync(
        string? search,
        bool? isActive,
        string? sortBy,
        bool descending,
        int pageNumber,
        int pageSize);

    Task<bool> ExistsAsync(Guid id);

    void Add(Customer customer);

    void Update(Customer customer);

    void Activate(Customer customer);

    void Deactivate(Customer customer);
}