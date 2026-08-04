using GawishERP.Application.Common.Results;
using GawishERP.Application.Features.Customers.DTOs;
using MediatR;

namespace GawishERP.Application.Features.Customers.Queries.GetCustomerById;

public class GetCustomerByIdQuery
    : IRequest<Result<CustomerDto>>
{
    public Guid Id { get; }

    public GetCustomerByIdQuery(Guid id)
    {
        Id = id;
    }
}