using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.FiscalYears.DTOs;
using MediatR;

namespace GawishERP.Application.Features.FiscalYears.Queries.GetFiscalYearById;

public sealed record GetFiscalYearByIdQuery(Guid Id)
    : IRequest<Result<FiscalYearDto>>;