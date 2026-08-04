using MediatR;

namespace GawishERP.Application.Features.Sales.Sales.Queries.GetList;

public sealed record GetSalesListQuery(
    string? Search,
    Guid? CustomerId,
    Guid? WarehouseId,
    string? Status,
    DateTime? FromDate,
    DateTime? ToDate,
    int PageNumber = 1,
    int PageSize = 20)
    : IRequest<GetSalesListResponse>;