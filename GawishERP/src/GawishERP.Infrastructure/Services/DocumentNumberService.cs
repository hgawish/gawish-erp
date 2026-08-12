using GawishERP.Application.Common.Interfaces;
using GawishERP.Domain.Common;
using GawishERP.Domain.Interfaces;

namespace GawishERP.Infrastructure.Services;

public sealed class DocumentNumberService
    : IDocumentNumberService
{
    private readonly INumberSeriesRepository _numberSeriesRepository;

    public DocumentNumberService(
        INumberSeriesRepository numberSeriesRepository)
    {
        _numberSeriesRepository = numberSeriesRepository;
    }

    // =========================================================
    // Generate Number
    // =========================================================

    public async Task<string> GenerateAsync(
        DocumentType documentType,
        CancellationToken cancellationToken = default)
    {
        var series =
            await _numberSeriesRepository.GetByDocumentTypeAsync(
                documentType);

        if (series is null)
        {
            throw new InvalidOperationException(
                $"Number Series for '{documentType}' was not found.");
        }

        // GetByDocumentTypeAsync returns a tracked entity.
        // EF Core automatically detects the CurrentNumber change,
        // so explicitly calling Update() is unnecessary and can
        // interfere with the RowVersion concurrency token.
        return series.GenerateNextNumber();
    }

    // =========================================================
    // Generate Number
    // Company / Branch / Fiscal Year
    // =========================================================

    public async Task<string> GenerateAsync(
        DocumentType documentType,
        Guid? companyId,
        Guid? branchId,
        Guid? fiscalYearId,
        CancellationToken cancellationToken = default)
    {
        var series =
            await _numberSeriesRepository.GetByDocumentTypeAsync(
                documentType,
                companyId,
                branchId,
                fiscalYearId);

        if (series is null)
        {
            throw new InvalidOperationException(
                $"Number Series for '{documentType}' was not found.");
        }

        // The repository query is tracked, therefore EF Core will
        // persist CurrentNumber automatically on SaveChangesAsync().
        return series.GenerateNextNumber();
    }
}