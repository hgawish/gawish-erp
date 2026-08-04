using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Features.Customers.Commands.UpdateCustomer;
using GawishERP.Domain.Interfaces;
using MediatR;

namespace GawishERP.Application.Features.Customers.Commands.Update;

public sealed class UpdateCustomerCommandHandler
    : IRequestHandler<UpdateCustomerCommand, Guid>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        IUnitOfWork unitOfWork)
    {
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        UpdateCustomerCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id);

        if (customer is null)
            throw new InvalidOperationException("Customer not found.");

        customer.Update(
            request.Name,
            request.ArabicName,
            request.Phone,
            request.Email,
            request.Address,
            request.Notes,
            request.AccountId);

        if (request.IsActive)
            customer.Activate();
        else
            customer.Deactivate();

        _customerRepository.Update(customer);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return customer.Id;
    }
}