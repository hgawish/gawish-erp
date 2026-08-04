using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class Customer : ActivatableEntity
{
    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? ArabicName { get; private set; }

    public string? Phone { get; private set; }

    public string? Email { get; private set; }

    public string? Address { get; private set; }

    public string? Notes { get; private set; }

    // ============================================
    // Accounting
    // ============================================

    public Guid? AccountId { get; private set; }

    public Account? Account { get; private set; }

    private Customer()
    {
    }

    public Customer(
        string code,
        string name,
        string? arabicName,
        string? phone,
        string? email,
        string? address,
        string? notes,
        Guid? accountId)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Customer code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Customer name is required.", nameof(name));

        Code = code.Trim();
        Name = name.Trim();
        ArabicName = arabicName?.Trim();
        Phone = phone?.Trim();
        Email = email?.Trim();
        Address = address?.Trim();
        Notes = notes?.Trim();

        AccountId = accountId;
    }

    public void Update(
        string name,
        string? arabicName,
        string? phone,
        string? email,
        string? address,
        string? notes,
        Guid? accountId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Customer name is required.", nameof(name));

        Name = name.Trim();
        ArabicName = arabicName?.Trim();
        Phone = phone?.Trim();
        Email = email?.Trim();
        Address = address?.Trim();
        Notes = notes?.Trim();

        AccountId = accountId;
    }

    public void SetAccount(Guid? accountId)
    {
        AccountId = accountId;
    }
}