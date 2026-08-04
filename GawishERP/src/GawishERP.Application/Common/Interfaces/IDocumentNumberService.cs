using GawishERP.Domain.Common;

namespace GawishERP.Application.Common.Interfaces;

public interface IDocumentNumberService
{
    Task<string> GenerateAsync(
        DocumentType documentType,
        CancellationToken cancellationToken = default);

    Task<string> GenerateAsync(
        DocumentType documentType,
        Guid? companyId,
        Guid? branchId,
        Guid? fiscalYearId,
        CancellationToken cancellationToken = default);
}