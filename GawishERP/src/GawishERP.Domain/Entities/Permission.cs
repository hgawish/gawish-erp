namespace GawishERP.Domain.Entities;

public class Permission
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }

    // Navigation Property
    public ICollection<RolePermission> RolePermissions { get; private set; } = new List<RolePermission>();

    private Permission()
    {
    }

    public Permission(
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