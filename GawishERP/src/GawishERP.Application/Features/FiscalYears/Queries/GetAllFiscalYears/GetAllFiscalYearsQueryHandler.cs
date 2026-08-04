using GawishERP.Application.Common.Mapping;
using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.FiscalYears.DTOs;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.FiscalYears.Queries.GetAllFiscalYears;

public sealed class GetAllFiscalYearsQueryHandler
    : IRequestHandler<GetAllFiscalYearsQuery, PagedResult<FiscalYearDto>>
{
    private readonly IFiscalYearRepository _fiscalYearRepository;

    public GetAllFiscalYearsQueryHandler(
        IFiscalYearRepository fiscalYearRepository)
    {
        _fiscalYearRepository = fiscalYearRepository;
    }

    public async Task<PagedResult<FiscalYearDto>> Handle(
        GetAllFiscalYearsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _fiscalYearRepository.GetAllAsync(
            request.Search,
            request.IsActive,
            request.IsOpen,
            request.SortBy,
            request.Descending,
            request.PageNumber,
            request.PageSize);

        var dto = FiscalYearMapper.ToDtoList(result.Items);

        return PagedResult<FiscalYearDto>.Create(
            dto,
            result.TotalCount,
            request.PageNumber,
            request.PageSize);
    }
}