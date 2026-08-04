using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Features.Customers.Commands.CreateCustomer;
using GawishERP.Domain.Entities;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Customers.Commands.CreateCustomer;
public sealed class CreateCustomerCommandHandler
    : IRequestHandler<CreateCustomerCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        CreateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = new GawishERP.Domain.Entities.Customer(
            request.Code,
            request.Name,
            request.ArabicName,
            request.Phone,
            request.Email,
            request.Address,
            request.Notes,
            request.AccountId);

        _customerRepository.Add(customer);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}