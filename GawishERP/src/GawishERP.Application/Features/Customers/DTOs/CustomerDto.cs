namespace GawishERP.Application.Features.Customers.DTOs;

public class CustomerDto
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? ArabicName { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? Address { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    // NEW
    public Guid? AccountId { get; set; }

    public string? AccountCode { get; set; }

    public string? AccountName { get; set; }
}