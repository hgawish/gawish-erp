using MediatR;

namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Queries.GetList;

public sealed record GetPurchaseReturnListQuery(
    string? Search = null,
    int PageNumber = 1,
    int PageSize = 20)
    : IRequest<List<PurchaseReturnListItemDto>>;