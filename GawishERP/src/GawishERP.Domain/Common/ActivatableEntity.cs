namespace GawishERP.Domain.Common;

public abstract class ActivatableEntity : AuditableEntity
{
    public bool IsActive { get; private set; } = true;

    public void Activate(Guid? userId = null)
    {
        if (IsActive)
            return;

        IsActive = true;
        MarkUpdated(userId);
    }

    public void Deactivate(Guid? userId = null)
    {
        if (!IsActive)
            return;

        IsActive = false;
        MarkUpdated(userId);
    }
}