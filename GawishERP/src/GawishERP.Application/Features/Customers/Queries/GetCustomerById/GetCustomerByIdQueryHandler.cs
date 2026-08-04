using GawishERP.Application.Common.Mapping;
using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Customers.DTOs;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQueryHandler
    : IRequestHandler<GetCustomerByIdQuery, Result<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerByIdQueryHandler(
        ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<CustomerDto>> Handle(
        GetCustomerByIdQuery request,
        CancellationToken cancellationToken)
    {
        var customer =
            await _customerRepository.GetByIdAsync(request.Id);

        if (customer is null)
        {
            return Result.Failure<CustomerDto>(
                new Error(
                    "Customer.NotFound",
                    $"Customer with Id '{request.Id}' was not found.",
                    ErrorType.NotFound));
        }

        return Result.Success(
            CustomerMapper.ToDto(customer));
    }
}