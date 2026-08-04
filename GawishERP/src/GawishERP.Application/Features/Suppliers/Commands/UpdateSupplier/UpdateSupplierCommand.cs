using MediatR;

namespace GawishERP.Application.Features.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommand : IRequest<Guid>
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? ArabicName { get; set; }

    public string? ContactPerson { get; set; }

    public string? Phone { get; set; }

    public string? Mobile { get; set; }

    public string? Email { get; set; }

    public string? TaxNumber { get; set; }

    public string? CommercialRegistration { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? Country { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    // ============================================
    // Accounting
    // ============================================

    public Guid? AccountId { get; set; }
}