using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public sealed class SalesQuotation : AuditableEntity
{
    public string QuotationNumber { get; private set; } = string.Empty;

    public DateTime QuotationDate { get; private set; }

    public Guid CustomerId { get; private set; }

    public Customer Customer { get; private set; } = null!;

    public Guid WarehouseId { get; private set; }

    public Warehouse Warehouse { get; private set; } = null!;

    public SalesQuotationStatus Status { get; private set; }

    public decimal SubTotal { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal TaxAmount { get; private set; }

    public decimal TotalAmount { get; private set; }

    public string? Remarks { get; private set; }

    private readonly List<SalesQuotationLine> _lines = new();

    public IReadOnlyCollection<SalesQuotationLine> Lines =>
        _lines.AsReadOnly();

    //====================================================
    // EF Core Constructor
    //====================================================

    private SalesQuotation()
    {
    }

    //====================================================
    // Constructor
    //====================================================

    public SalesQuotation(
        string quotationNumber,
        DateTime quotationDate,
        Guid customerId,
        Guid warehouseId,
        string? remarks = null)
    {
        if (string.IsNullOrWhiteSpace(quotationNumber))
            throw new ArgumentException(
                "Quotation number cannot be empty.",
                nameof(quotationNumber));

        if (customerId == Guid.Empty)
            throw new ArgumentException(
                "Customer ID cannot be empty.",
                nameof(customerId));

        if (warehouseId == Guid.Empty)
            throw new ArgumentException(
                "Warehouse ID cannot be empty.",
                nameof(warehouseId));

        QuotationNumber = quotationNumber;

        QuotationDate = quotationDate;

        CustomerId = customerId;

        WarehouseId = warehouseId;

        Remarks = remarks;

        Status = SalesQuotationStatus.Draft;

        Recalculate();
    }

    //====================================================
    // Lines
    //====================================================

    public void AddLine(
        Guid productId,
        decimal quantity,
        decimal unitPrice,
        decimal discountPercent = 0,
        decimal taxPercent = 0)
    {
        if (Status != SalesQuotationStatus.Draft)
            throw new InvalidOperationException(
                "Only Draft quotations can be modified.");

        var line = new SalesQuotationLine(
            Id,
            productId,
            quantity,
            unitPrice,
            discountPercent,
            taxPercent);

        _lines.Add(line);

        Recalculate();
    }

    public void RemoveLine(Guid lineId)
    {
        if (Status != SalesQuotationStatus.Draft)
            throw new InvalidOperationException(
                "Only Draft quotations can be modified.");

        var line = _lines.FirstOrDefault(
            x => x.Id == lineId);

        if (line is null)
            return;

        _lines.Remove(line);

        Recalculate();
    }

    //====================================================
    // Workflow
    //====================================================

    public void Submit()
    {
        if (Status != SalesQuotationStatus.Draft)
            throw new InvalidOperationException(
                "Only Draft quotations can be submitted.");

        if (!_lines.Any())
            throw new InvalidOperationException(
                "Sales quotation has no lines.");

        Recalculate();

        Status = SalesQuotationStatus.Submitted;
    }

    public void Approve()
    {
        if (Status != SalesQuotationStatus.Submitted)
            throw new InvalidOperationException(
                "Only submitted quotations can be approved.");

        Status = SalesQuotationStatus.Approved;
    }

    public void Reject()
    {
        if (Status != SalesQuotationStatus.Submitted)
            throw new InvalidOperationException(
                "Only submitted quotations can be rejected.");

        Status = SalesQuotationStatus.Rejected;
    }

    public void Cancel()
    {
        if (Status == SalesQuotationStatus.Cancelled)
            return;

        Status = SalesQuotationStatus.Cancelled;
    }

    //====================================================
    // Calculations
    //====================================================

    private void Recalculate()
    {
        SubTotal =
            _lines.Sum(x => x.LineSubTotal);

        DiscountAmount =
            _lines.Sum(x => x.DiscountAmount);

        TaxAmount =
            _lines.Sum(x => x.TaxAmount);

        TotalAmount =
            _lines.Sum(x => x.LineTotal);
    }
}