using MediatR;

namespace GawishERP.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ArabicName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    // NEW
    public Guid? AccountId { get; set; }
}