using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Interfaces;

public interface INumberSeriesRepository
{
    Task<NumberSeries?> GetByIdAsync(Guid id);

    Task<NumberSeries?> GetByDocumentTypeAsync(
        DocumentType documentType,
        Guid? companyId = null,
        Guid? branchId = null,
        Guid? fiscalYearId = null);

    Task<(List<NumberSeries> Items, int TotalCount)> GetAllAsync(
        string? search,
        bool? isActive,
        DocumentType? documentType,
        int pageNumber,
        int pageSize);

    Task<bool> ExistsAsync(Guid id);

    /// <summary>
    /// Atomically generates the next document number and increments CurrentNumber in the database.
    /// The NumberSeries entity is not tracked as Modified by this operation.
    /// </summary>
    Task<string> GetNextNumberAsync(
        DocumentType documentType,
        Guid? companyId = null,
        Guid? branchId = null,
        Guid? fiscalYearId = null);

    void Add(NumberSeries numberSeries);

    void Update(NumberSeries numberSeries);

    void Activate(NumberSeries numberSeries);

    void Deactivate(NumberSeries numberSeries);
}