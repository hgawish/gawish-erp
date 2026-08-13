using GawishERP.Domain.Entities;

namespace GawishERP.Domain.Common;

public abstract class BaseDocumentEntity : BaseEntity
{
    public string DocumentNumber { get; protected set; } = string.Empty;

    public DateTime DocumentDate { get; protected set; }

    public DocumentStatus Status { get; protected set; }

    public string? Notes { get; protected set; }

    //=========================================================
    // Multi Company
    //=========================================================

    public Guid FiscalYearId { get; protected set; }

    public Guid? CompanyId { get; protected set; }

    public Guid? BranchId { get; protected set; }

    protected BaseDocumentEntity()
    {
    }

    public virtual void Submit()
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Only draft documents can be submitted.");

        Status = DocumentStatus.Submitted;
    }

    public virtual void Approve()
    {
        if (Status != DocumentStatus.Submitted)
            throw new InvalidOperationException(
                "Only submitted documents can be approved.");

        Status = DocumentStatus.Approved;
    }

    public virtual void Post()
    {
        if (Status != DocumentStatus.Approved)
            throw new InvalidOperationException(
                "Only approved documents can be posted.");

        Status = DocumentStatus.Posted;
    }

    public virtual void Cancel()
    {
        if (Status == DocumentStatus.Cancelled)
            throw new InvalidOperationException(
                "Document is already cancelled.");

        if (Status != DocumentStatus.Posted)
            throw new InvalidOperationException(
                "Only posted documents can be cancelled.");

        Status = DocumentStatus.Cancelled;
    }

    public void AssignOrganization(
        Guid fiscalYearId,
        Guid? companyId,
        Guid? branchId)
    {
        FiscalYearId = fiscalYearId;

        CompanyId = companyId;

        BranchId = branchId;
    }
}