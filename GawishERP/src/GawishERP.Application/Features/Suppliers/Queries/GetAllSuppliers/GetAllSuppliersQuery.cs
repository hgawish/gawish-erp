using GawishERP.Application.Common.Pagination;
using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Suppliers.DTOs;
using MediatR;

namespace GawishERP.Application.Features.Suppliers.Queries.GetAllSuppliers;

public class GetAllSuppliersQuery
    : PaginationRequest,
      IRequest<PagedResult<SupplierDto>>
{
    public bool? IsActive { get; set; }
}