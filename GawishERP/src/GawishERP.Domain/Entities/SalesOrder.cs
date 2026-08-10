using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public sealed class SalesOrder : BaseDocumentEntity
{
    private readonly List<SalesOrderLine> _lines = new();

    public Guid CustomerId { get; private set; }

    public Guid? SalesQuotationId { get; private set; }

    public decimal TotalBeforeDiscount { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal TotalAfterDiscount { get; private set; }

    public decimal TaxAmount { get; private set; }

    public decimal NetAmount { get; private set; }

    //====================================================
    // Navigation
    //====================================================

    public Customer Customer { get; private set; } = null!;

    public SalesQuotation? SalesQuotation { get; private set; }

    public IReadOnlyCollection<SalesOrderLine> Lines =>
        _lines.AsReadOnly();

    //====================================================
    // EF Core Constructor
    //====================================================

    private SalesOrder()
    {
    }

    //====================================================
    // Constructor
    //====================================================

    public SalesOrder(
        string documentNumber,
        DateTime documentDate,
        Guid fiscalYearId,
        Guid customerId,
        Guid? salesQuotationId,
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

        if (customerId == Guid.Empty)
            throw new ArgumentException(
                "Customer ID cannot be empty.",
                nameof(customerId));

        if (salesQuotationId == Guid.Empty)
            salesQuotationId = null;

        DocumentNumber = documentNumber;

        DocumentDate = documentDate;

        FiscalYearId = fiscalYearId;

        CustomerId = customerId;

        SalesQuotationId = salesQuotationId;

        CompanyId = companyId;

        BranchId = branchId;

        Notes = notes;

        Status = DocumentStatus.Draft;

        RecalculateTotals();
    }

    //====================================================
    // Lines
    //====================================================

    public void AddLine(
        Guid productId,
        Guid warehouseId,
        decimal quantity,
        decimal unitPrice,
        decimal discountPercent = 0,
        decimal taxPercent = 0)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Only Draft Sales Orders can be modified.");

        var line = new SalesOrderLine(
            Id,
            productId,
            warehouseId,
            quantity,
            unitPrice,
            discountPercent,
            taxPercent);

        _lines.Add(line);

        RecalculateTotals();
    }

    public void RemoveLine(Guid lineId)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Only Draft Sales Orders can be modified.");

        var line = _lines.FirstOrDefault(
            x => x.Id == lineId);

        if (line is null)
            return;

        _lines.Remove(line);

        RecalculateTotals();
    }

    public void ClearLines()
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Only Draft Sales Orders can be modified.");

        _lines.Clear();

        RecalculateTotals();
    }

    //====================================================
    // Notes
    //====================================================

    public void SetNotes(string? notes)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Only Draft Sales Orders can be modified.");

        Notes = notes;
    }

    //====================================================
    // Totals
    //====================================================

    private void RecalculateTotals()
    {
        TotalBeforeDiscount =
            _lines.Sum(x => x.LineTotalBeforeDiscount);

        DiscountAmount =
            _lines.Sum(x => x.DiscountAmount);

        TotalAfterDiscount =
            _lines.Sum(x => x.LineTotalAfterDiscount);

        TaxAmount =
            _lines.Sum(x => x.TaxAmount);

        NetAmount =
            _lines.Sum(x => x.NetAmount);
    }

    //====================================================
    // Workflow
    //====================================================

    public override void Submit()
    {
        if (!_lines.Any())
            throw new InvalidOperationException(
                "Sales Order has no lines.");

        RecalculateTotals();

        base.Submit();
    }

    public override void Approve()
    {
        if (Status != DocumentStatus.Submitted)
            throw new InvalidOperationException(
                "Only submitted Sales Orders can be approved.");

        base.Approve();
    }

    public override void Post()
    {
        if (Status != DocumentStatus.Approved)
            throw new InvalidOperationException(
                "Only approved Sales Orders can be posted.");

        RecalculateTotals();

        base.Post();
    }

    public override void Cancel()
    {
        base.Cancel();
    }
}