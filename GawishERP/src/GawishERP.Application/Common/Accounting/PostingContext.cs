using GawishERP.Domain.Common;

namespace GawishERP.Application.Common.Accounting;

public sealed class PostingContext
{
    public Guid FiscalYearId { get; init; }

    public Guid? CompanyId { get; init; }

    public Guid? BranchId { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public DocumentType DocumentType { get; init; }

    public DateTime PostingDate { get; init; }

    public string Description { get; init; } = string.Empty;
}