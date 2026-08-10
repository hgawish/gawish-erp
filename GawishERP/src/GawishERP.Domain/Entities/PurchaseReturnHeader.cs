using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class PurchaseReturnHeader : BaseDocumentEntity
{
    private readonly List<PurchaseReturnLine> _lines = new();

    public Guid PurchaseId { get; private set; }

    public Guid SupplierId { get; private set; }

    public Guid WarehouseId { get; private set; }

    public string ReturnReason { get; private set; } = string.Empty;

    public decimal TotalAmount { get; private set; }

    //=========================================================
    // Navigation
    //=========================================================

    public PurchaseHeader Purchase { get; private set; } = null!;

    public Supplier Supplier { get; private set; } = null!;

    public Warehouse Warehouse { get; private set; } = null!;

    public IReadOnlyCollection<PurchaseReturnLine> Lines =>
        _lines.AsReadOnly();

    private PurchaseReturnHeader()
    {
    }

    public PurchaseReturnHeader(
        string documentNumber,
        DateTime documentDate,

        Guid fiscalYearId,
        Guid? companyId,
        Guid? branchId,

        Guid purchaseId,
        Guid supplierId,
        Guid warehouseId,

        string returnReason,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new ArgumentException(nameof(documentNumber));

        if (purchaseId == Guid.Empty)
            throw new ArgumentException(nameof(purchaseId));

        if (supplierId == Guid.Empty)
            throw new ArgumentException(nameof(supplierId));

        if (warehouseId == Guid.Empty)
            throw new ArgumentException(nameof(warehouseId));

        DocumentNumber = documentNumber;

        DocumentDate = documentDate;

        AssignOrganization(
            fiscalYearId,
            companyId,
            branchId);

        PurchaseId = purchaseId;

        SupplierId = supplierId;

        WarehouseId = warehouseId;

        ReturnReason = returnReason;

        Notes = notes;

        Status = DocumentStatus.Draft;
    }

    public void AddLine(
        Guid purchaseLineId,
        Guid productId,
        decimal quantity,
        decimal unitCost,
        string? notes)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Cannot modify posted document.");

        var line = new PurchaseReturnLine(
            purchaseLineId,
            productId,
            quantity,
            unitCost,
            notes);

        _lines.Add(line);

        Recalculate();
    }

    public void RemoveLine(Guid lineId)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Cannot modify posted document.");

        var line = _lines.FirstOrDefault(x => x.Id == lineId);

        if (line is null)
            return;

        _lines.Remove(line);

        Recalculate();
    }

    private void Recalculate()
    {
        TotalAmount =
            _lines.Sum(x => x.LineTotal);
    }

    public override void Post()
    {
        if (!_lines.Any())
            throw new InvalidOperationException(
                "Document has no lines.");

        Recalculate();

        if (TotalAmount <= 0)
            throw new InvalidOperationException(
                "Return total must be greater than zero.");

        base.Post();
    }

    public override void Cancel()
    {
        base.Cancel();
    }
}