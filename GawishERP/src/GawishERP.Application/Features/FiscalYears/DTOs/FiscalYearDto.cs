namespace GawishERP.Application.Features.FiscalYears.DTOs;

public class FiscalYearDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public bool IsOpen { get; set; }

    public bool IsClosed { get; set; }

    public bool IsActive { get; set; }
}