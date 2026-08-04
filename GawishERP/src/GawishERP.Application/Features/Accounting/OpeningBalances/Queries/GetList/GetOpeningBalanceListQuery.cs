using GawishERP.Application.Common.Results;
using GawishERP.Domain.Common;
using MediatR;

namespace GawishERP.Application.Features.Accounting.OpeningBalances.Queries.GetList;

public sealed record GetOpeningBalanceListQuery(
    string? Search,
    DateTime? FromDate,
    DateTime? ToDate,
    DocumentStatus? Status,
    int PageNumber = 1,
    int PageSize = 20)
    : IRequest<Result<PagedResult<OpeningBalanceListDto>>>;