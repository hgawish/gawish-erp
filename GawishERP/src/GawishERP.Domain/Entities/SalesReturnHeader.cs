using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class SalesReturnHeader : BaseDocumentEntity
{
    private readonly List<SalesReturnLine> _lines = new();

    public Guid SalesId { get; private set; }

    public Guid CustomerId { get; private set; }

    public Guid WarehouseId { get; private set; }

    public string ReturnReason { get; private set; } = string.Empty;

    public decimal TotalAmount { get; private set; }

    // Navigation

    public SalesHeader Sales { get; private set; } = null!;

    public Customer Customer { get; private set; } = null!;

    public Warehouse Warehouse { get; private set; } = null!;

    public IReadOnlyCollection<SalesReturnLine> Lines =>
        _lines.AsReadOnly();

    private SalesReturnHeader()
    {
    }

    public SalesReturnHeader(
        string documentNumber,
        DateTime documentDate,

        Guid fiscalYearId,
        Guid? companyId,
        Guid? branchId,

        Guid salesId,
        Guid customerId,
        Guid warehouseId,

        string returnReason,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new ArgumentException(nameof(documentNumber));

        if (salesId == Guid.Empty)
            throw new ArgumentException(nameof(salesId));

        if (customerId == Guid.Empty)
            throw new ArgumentException(nameof(customerId));

        if (warehouseId == Guid.Empty)
            throw new ArgumentException(nameof(warehouseId));

        DocumentNumber = documentNumber;
        DocumentDate = documentDate;

        FiscalYearId = fiscalYearId;
        CompanyId = companyId;
        BranchId = branchId;

        SalesId = salesId;
        CustomerId = customerId;
        WarehouseId = warehouseId;

        ReturnReason = returnReason;

        Notes = notes;

        Status = DocumentStatus.Draft;
    }

    public void AddLine(
        Guid salesLineId,
        Guid productId,
        decimal quantity,
        decimal unitPrice,
        string? notes)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Cannot modify posted document.");

        var line = new SalesReturnLine(
            salesLineId,
            productId,
            quantity,
            unitPrice,
            notes);

        _lines.Add(line);

        Recalculate();
    }

    public void RemoveLine(Guid lineId)
    {
        var line = _lines.FirstOrDefault(x => x.Id == lineId);

        if (line is null)
            return;

        _lines.Remove(line);

        Recalculate();
    }

    private void Recalculate()
    {
        TotalAmount = _lines.Sum(x => x.LineTotal);
    }

    public override void Post()
    {
        if (!_lines.Any())
            throw new InvalidOperationException(
                "Document has no lines.");

        Recalculate();

        base.Post();
    }

    public override void Cancel()
    {
        base.Cancel();
    }
}