using GawishERP.Application.Features.FiscalYears.DTOs;
using GawishERP.Domain.Entities;

namespace GawishERP.Application.Common.Mapping;

public static class FiscalYearMapper
{
    public static FiscalYearDto ToDto(FiscalYear fiscalYear)
    {
        return new FiscalYearDto
        {
            Id = fiscalYear.Id,
            Code = fiscalYear.Code,
            Name = fiscalYear.Name,
            StartDate = fiscalYear.StartDate,
            EndDate = fiscalYear.EndDate,
            IsOpen = fiscalYear.IsOpen,
            IsClosed = fiscalYear.IsClosed,
            IsActive = fiscalYear.IsActive
        };
    }

    public static List<FiscalYearDto> ToDtoList(
        IEnumerable<FiscalYear> fiscalYears)
    {
        return fiscalYears
            .Select(ToDto)
            .ToList();
    }
}