namespace GawishERP.Application.Features.Suppliers.DTOs;

public class SupplierDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

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

    public string? AccountCode { get; set; }

    public string? AccountName { get; set; }
}