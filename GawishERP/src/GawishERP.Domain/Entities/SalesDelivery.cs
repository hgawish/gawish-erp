using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public sealed class SalesDelivery : BaseDocumentEntity
{
    private readonly List<SalesDeliveryLine> _lines = new();

    public Guid SalesOrderId { get; private set; }

    public Guid CustomerId { get; private set; }

    public decimal TotalQuantity { get; private set; }

    //====================================================
    // Navigation
    //====================================================

    public SalesOrder SalesOrder { get; private set; } = null!;

    public Customer Customer { get; private set; } = null!;

    public IReadOnlyCollection<SalesDeliveryLine> Lines =>
        _lines.AsReadOnly();

    //====================================================
    // EF Core Constructor
    //====================================================

    private SalesDelivery()
    {
    }

    //====================================================
    // Constructor
    //====================================================

    public SalesDelivery(
        string documentNumber,
        DateTime documentDate,
        Guid fiscalYearId,
        Guid salesOrderId,
        Guid customerId,
        Guid? companyId,
        Guid? branchId,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new ArgumentException(
                "Document number cannot be empty.",
                nameof(documentNumber));

        if (fiscalYearId == Guid.Empty)
            throw new ArgumentException(
                "Fiscal year ID cannot be empty.",
                nameof(fiscalYearId));

        if (salesOrderId == Guid.Empty)
            throw new ArgumentException(
                "Sales Order ID cannot be empty.",
                nameof(salesOrderId));

        if (customerId == Guid.Empty)
            throw new ArgumentException(
                "Customer ID cannot be empty.",
                nameof(customerId));

        DocumentNumber = documentNumber;
        DocumentDate = documentDate;

        FiscalYearId = fiscalYearId;

        SalesOrderId = salesOrderId;
        CustomerId = customerId;

        CompanyId = companyId;
        BranchId = branchId;

        Notes = notes;

        Status = DocumentStatus.Draft;

        Recalculate();
    }

    //====================================================
    // Lines
    //====================================================

    public void AddLine(
        Guid salesOrderLineId,
        Guid productId,
        Guid warehouseId,
        decimal quantity)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Only Draft Sales Deliveries can be modified.");

        var line = new SalesDeliveryLine(
            Id,
            salesOrderLineId,
            productId,
            warehouseId,
            quantity);

        _lines.Add(line);

        Recalculate();
    }

    public void RemoveLine(Guid lineId)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Only Draft Sales Deliveries can be modified.");

        var line = _lines.FirstOrDefault(
            x => x.Id == lineId);

        if (line is null)
            return;

        _lines.Remove(line);

        Recalculate();
    }

    private void Recalculate()
    {
        TotalQuantity =
            _lines.Sum(x => x.Quantity);
    }

    //====================================================
    // Workflow
    //====================================================

    public override void Submit()
    {
        if (!_lines.Any())
            throw new InvalidOperationException(
                "Sales Delivery has no lines.");

        Recalculate();

        base.Submit();
    }

    public override void Approve()
    {
        if (Status != DocumentStatus.Submitted)
            throw new InvalidOperationException(
                "Only submitted Sales Deliveries can be approved.");

        base.Approve();
    }

    public override void Post()
    {
        if (Status != DocumentStatus.Approved)
            throw new InvalidOperationException(
                "Only approved Sales Deliveries can be posted.");

        Recalculate();

        base.Post();
    }

    public override void Cancel()
    {
        base.Cancel();
    }
}