using GawishERP.Application.Common.Mapping;
using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Customers.DTOs;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Customers.Queries.GetAllCustomers;

public class GetAllCustomersQueryHandler
    : IRequestHandler<GetAllCustomersQuery, PagedResult<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetAllCustomersQueryHandler(
        ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<PagedResult<CustomerDto>> Handle(
        GetAllCustomersQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _customerRepository.GetAllAsync(
            request.Search,
            request.IsActive,
            request.SortBy,
            request.Descending,
            request.PageNumber,
            request.PageSize);

        var items = CustomerMapper.ToDtoList(result.Items);

        return new PagedResult<CustomerDto>(
            items,
            result.TotalCount,
            request.PageNumber,
            request.PageSize);
    }
}