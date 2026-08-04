using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class Warehouse : ActivatableEntity
{
    public string Code { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string? ArabicName { get; private set; }

    public string? Manager { get; private set; }

    public string? Phone { get; private set; }

    public string? Address { get; private set; }

    public string? Notes { get; private set; }

    private Warehouse()
    {
    }

    public Warehouse(
        string code,
        string name,
        string? arabicName,
        string? manager,
        string? phone,
        string? address,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Warehouse code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Warehouse name is required.", nameof(name));

        Code = code.Trim();
        Name = name.Trim();
        ArabicName = arabicName?.Trim();
        Manager = manager?.Trim();
        Phone = phone?.Trim();
        Address = address?.Trim();
        Notes = notes?.Trim();
    }

    public void Update(
        string name,
        string? arabicName,
        string? manager,
        string? phone,
        string? address,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Warehouse name is required.", nameof(name));

        Name = name.Trim();
        ArabicName = arabicName?.Trim();
        Manager = manager?.Trim();
        Phone = phone?.Trim();
        Address = address?.Trim();
        Notes = notes?.Trim();
    }
}