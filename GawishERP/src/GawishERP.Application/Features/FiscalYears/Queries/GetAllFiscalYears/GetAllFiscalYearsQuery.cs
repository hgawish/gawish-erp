using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.FiscalYears.DTOs;
using MediatR;

namespace GawishERP.Application.Features.FiscalYears.Queries.GetAllFiscalYears;

public sealed class GetAllFiscalYearsQuery
    : IRequest<PagedResult<FiscalYearDto>>
{
    public string? Search { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsOpen { get; set; }

    public string? SortBy { get; set; } = "StartDate";

    public bool Descending { get; set; }

    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;
}