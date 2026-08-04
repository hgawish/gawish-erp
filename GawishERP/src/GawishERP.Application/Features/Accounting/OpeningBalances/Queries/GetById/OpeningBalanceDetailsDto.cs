namespace GawishERP.Application.Features.Accounting.OpeningBalances.Queries.GetById;

using GawishERP.Domain.Common;

public sealed class OpeningBalanceDetailsDto
{
    public Guid Id { get; set; }

    public string DocumentNumber { get; set; } = string.Empty;

    public DateTime DocumentDate { get; set; }

    public Guid FiscalYearId { get; set; }

    public Guid? CompanyId { get; set; }

    public Guid? BranchId { get; set; }

    public string ReferenceNumber { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public DocumentStatus Status { get; set; }

    public decimal TotalDebit { get; set; }

    public decimal TotalCredit { get; set; }

    public List<OpeningBalanceLineDetailsDto> Lines { get; set; } = [];
}

public sealed class OpeningBalanceLineDetailsDto
{
    public Guid AccountId { get; set; }

    public string AccountCode { get; set; } = string.Empty;

    public string AccountName { get; set; } = string.Empty;

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public string Description { get; set; } = string.Empty;
}