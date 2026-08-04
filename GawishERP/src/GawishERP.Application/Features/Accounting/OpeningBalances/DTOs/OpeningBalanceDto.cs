namespace GawishERP.Application.Features.Accounting.OpeningBalances.DTOs;

public sealed class OpeningBalanceDto
{
    public Guid FiscalYearId { get; set; }

    public Guid? CompanyId { get; set; }

    public Guid? BranchId { get; set; }

    public DateTime DocumentDate { get; set; }

    public string ReferenceNumber { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public List<OpeningBalanceLineDto> Lines { get; set; } = [];
}