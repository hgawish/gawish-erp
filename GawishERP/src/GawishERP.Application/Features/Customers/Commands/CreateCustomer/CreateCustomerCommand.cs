using MediatR;

namespace GawishERP.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommand : IRequest<Guid>
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? ArabicName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Notes { get; set; }

    // NEW
    public Guid? AccountId { get; set; }
}