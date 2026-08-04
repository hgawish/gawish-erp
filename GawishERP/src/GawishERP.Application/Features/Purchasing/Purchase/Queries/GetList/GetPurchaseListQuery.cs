using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Purchasing.Purchase.Queries.GetList;

public sealed record GetPurchaseListQuery
    : IRequest<PagedResult<PurchaseListItemDto>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 20;

    public string? Search { get; init; }

    public string? SortBy { get; init; }

    public bool Descending { get; init; }
}