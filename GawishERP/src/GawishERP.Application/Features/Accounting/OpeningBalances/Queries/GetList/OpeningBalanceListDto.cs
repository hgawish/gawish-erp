using GawishERP.Domain.Common;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Queries.GetList;

public sealed class OpeningBalanceListDto
{
    public Guid Id { get; set; }

    public string DocumentNumber { get; set; } = string.Empty;

    public DateTime DocumentDate { get; set; }

    public Guid FiscalYearId { get; set; }

    public Guid? CompanyId { get; set; }

    public Guid? BranchId { get; set; }

    public string ReferenceNumber { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public decimal TotalDebit { get; set; }

    public decimal TotalCredit { get; set; }

    public DocumentStatus Status { get; set; }
}