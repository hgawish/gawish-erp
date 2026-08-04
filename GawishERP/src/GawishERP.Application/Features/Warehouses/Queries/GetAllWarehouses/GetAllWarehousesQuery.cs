using GawishERP.Application.Common.Pagination;
using GawishERP.Application.Common.Results;
using MediatR;

namespace GawishERP.Application.Features.Warehouses.Queries.GetAllWarehouses;

public class GetAllWarehousesQuery
    : PaginationRequest,
      IRequest<PagedResult<WarehouseDto>>
{
    public bool? IsActive { get; set; }
}