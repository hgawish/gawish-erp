using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class Supplier : ActivatableEntity
{
    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? ArabicName { get; private set; }

    public string? ContactPerson { get; private set; }

    public string? Phone { get; private set; }

    public string? Mobile { get; private set; }

    public string? Email { get; private set; }

    public string? TaxNumber { get; private set; }

    public string? CommercialRegistration { get; private set; }

    public string? Address { get; private set; }

    public string? City { get; private set; }

    public string? Country { get; private set; }

    public string? Notes { get; private set; }

    // ============================================
    // Accounting
    // ============================================

    public Guid? AccountId { get; private set; }

    public Account? Account { get; private set; }

    private Supplier()
    {
    }

    public Supplier(
        string code,
        string name,
        string? arabicName,
        string? contactPerson,
        string? phone,
        string? mobile,
        string? email,
        string? taxNumber,
        string? commercialRegistration,
        string? address,
        string? city,
        string? country,
        string? notes,
        Guid? accountId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Supplier code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Supplier name is required.", nameof(name));

        Code = code.Trim();
        Name = name.Trim();
        ArabicName = arabicName?.Trim();
        ContactPerson = contactPerson?.Trim();
        Phone = phone?.Trim();
        Mobile = mobile?.Trim();
        Email = email?.Trim();
        TaxNumber = taxNumber?.Trim();
        CommercialRegistration = commercialRegistration?.Trim();
        Address = address?.Trim();
        City = city?.Trim();
        Country = country?.Trim();
        Notes = notes?.Trim();

        AccountId = accountId;
    }

    public void Update(
        string name,
        string? arabicName,
        string? contactPerson,
        string? phone,
        string? mobile,
        string? email,
        string? taxNumber,
        string? commercialRegistration,
        string? address,
        string? city,
        string? country,
        string? notes,
        Guid? accountId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Supplier name is required.", nameof(name));

        Name = name.Trim();
        ArabicName = arabicName?.Trim();
        ContactPerson = contactPerson?.Trim();
        Phone = phone?.Trim();
        Mobile = mobile?.Trim();
        Email = email?.Trim();
        TaxNumber = taxNumber?.Trim();
        CommercialRegistration = commercialRegistration?.Trim();
        Address = address?.Trim();
        City = city?.Trim();
        Country = country?.Trim();
        Notes = notes?.Trim();

        AccountId = accountId;
    }

    public void SetAccount(Guid? accountId)
    {
        AccountId = accountId;
    }
}