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

    public async Task<string> GenerateAsync(
        DocumentType documentType,
        CancellationToken cancellationToken = default)
    {
        return await _numberSeriesRepository.GetNextNumberAsync(documentType);
    }

    public async Task<string> GenerateAsync(
        DocumentType documentType,
        Guid? companyId,
        Guid? branchId,
        Guid? fiscalYearId,
        CancellationToken cancellationToken = default)
    {
        return await _numberSeriesRepository.GetNextNumberAsync(
            documentType,
            companyId,
            branchId,
            fiscalYearId);
    }
}