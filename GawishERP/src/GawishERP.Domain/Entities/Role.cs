namespace GawishERP.Domain.Entities;

public class Role
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    // Navigation Properties
    public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    private Role()
    {
    }

    public Role(
        string name,
        string description)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        IsActive = true;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Update(
        string name,
        string description)
    {
        Name = name;
        Description = description;
    }
}