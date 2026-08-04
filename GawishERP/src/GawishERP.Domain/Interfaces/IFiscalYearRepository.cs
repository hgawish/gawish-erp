using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface IFiscalYearRepository
{
    Task<FiscalYear?> GetByIdAsync(Guid id);

    Task<FiscalYear?> GetByCodeAsync(string code);

    Task<FiscalYear?> GetOpenFiscalYearAsync();

    Task<(List<FiscalYear> Items, int TotalCount)> GetAllAsync(
        string? search,
        bool? isActive,
        bool? isOpen,
        string? sortBy,
        bool descending,
        int pageNumber,
        int pageSize);

    Task<bool> ExistsAsync(Guid id);

    void Add(FiscalYear fiscalYear);

    void Update(FiscalYear fiscalYear);

    void Activate(FiscalYear fiscalYear);

    void Deactivate(FiscalYear fiscalYear);
}