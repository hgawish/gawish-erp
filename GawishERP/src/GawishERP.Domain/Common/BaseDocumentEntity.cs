using GawishERP.Domain.Common;

namespace GawishERP.Domain.Common;

public abstract class BaseDocumentEntity : BaseEntity
{
    public string DocumentNumber { get; protected set; } = string.Empty;

    public DateTime DocumentDate { get; protected set; }

    public DocumentStatus Status { get; protected set; }
        = DocumentStatus.Draft;

    public string? Notes { get; protected set; }

    public virtual void Submit()
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Only Draft documents can be submitted.");

        Status = DocumentStatus.Submitted;
    }

    public virtual void Approve()
    {
        if (Status != DocumentStatus.Submitted)
            throw new InvalidOperationException(
                "Only Submitted documents can be approved.");

        Status = DocumentStatus.Approved;
    }

    public virtual void Post()
    {
        if (Status != DocumentStatus.Approved)
            throw new InvalidOperationException(
                "Only Approved documents can be posted.");

        Status = DocumentStatus.Posted;
    }

    public virtual void Cancel()
    {
        if (Status == DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Posted document cannot be cancelled.");

        Status = DocumentStatus.Cancelled;
    }
}