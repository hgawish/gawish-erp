using GawishERP.Application.Common.Pagination;
using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Inventory.OpeningBalance.Queries.GetList;

public sealed class GetOpeningBalanceListQuery
    : PaginationRequest,
      IRequest<PagedResult<OpeningBalanceListItemDto>>
{
}