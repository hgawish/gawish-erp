using GawishERP.Application.Common.Mapping;
using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.FiscalYears.DTOs;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.FiscalYears.Queries.GetFiscalYearById;

public sealed class GetFiscalYearByIdQueryHandler
    : IRequestHandler<GetFiscalYearByIdQuery, Result<FiscalYearDto>>
{
    private readonly IFiscalYearRepository _fiscalYearRepository;

    public GetFiscalYearByIdQueryHandler(
        IFiscalYearRepository fiscalYearRepository)
    {
        _fiscalYearRepository = fiscalYearRepository;
    }

    public async Task<Result<FiscalYearDto>> Handle(
        GetFiscalYearByIdQuery request,
        CancellationToken cancellationToken)
    {
        var fiscalYear =
            await _fiscalYearRepository.GetByIdAsync(request.Id);

        if (fiscalYear is null)
        {
            return Result.Failure<FiscalYearDto>(
                new Error(
                    "FiscalYear.NotFound",
                    $"Fiscal Year '{request.Id}' was not found.",
                    ErrorType.NotFound));
        }

        return Result.Success(
            FiscalYearMapper.ToDto(fiscalYear));
    }
}