using GawishERP.Application.Common.Pagination;
using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Customers.DTOs;
using MediatR;

namespace GawishERP.Application.Features.Customers.Queries.GetAllCustomers;

public class GetAllCustomersQuery
    : PaginationRequest,
      IRequest<PagedResult<CustomerDto>>
{
    public bool? IsActive { get; set; }
}