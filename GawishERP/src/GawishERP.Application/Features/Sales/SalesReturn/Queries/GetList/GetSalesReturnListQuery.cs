using MediatR;

namespace GawishERP.Application.Features.Sales.SalesReturn.Queries.GetList;

public sealed record GetSalesReturnListQuery(
    string? Search,
    Guid? CustomerId,
    Guid? WarehouseId,
    DateTime? FromDate,
    DateTime? ToDate,
    int PageNumber = 1,
    int PageSize = 20)
    : IRequest<GetSalesReturnListResponse>;