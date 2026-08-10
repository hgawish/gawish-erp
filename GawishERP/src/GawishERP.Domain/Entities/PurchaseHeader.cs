using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class PurchaseHeader : BaseDocumentEntity
{
    private readonly List<PurchaseLine> _lines = new();

    public string InvoiceNumber { get; private set; } = string.Empty;

    public DateTime InvoiceDate { get; private set; }

    public Guid SupplierId { get; private set; }

    public Guid WarehouseId { get; private set; }

    public string Currency { get; private set; } = "EGP";

    public decimal ExchangeRate { get; private set; } = 1;

    public decimal TotalBeforeDiscount { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal TaxAmount { get; private set; }

    public decimal NetTotal { get; private set; }

    //==========================================================
    // Navigation
    //==========================================================

    public Supplier Supplier { get; private set; } = null!;

    public Warehouse Warehouse { get; private set; } = null!;

    public IReadOnlyCollection<PurchaseLine> Lines =>
        _lines.AsReadOnly();

    private PurchaseHeader()
    {
    }

    public PurchaseHeader(
        string documentNumber,
        DateTime documentDate,

        Guid fiscalYearId,
        Guid? companyId,
        Guid? branchId,

        string invoiceNumber,
        DateTime invoiceDate,

        Guid supplierId,
        Guid warehouseId,

        string currency,
        decimal exchangeRate,

        string? notes)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new ArgumentException(
                "Document Number is required.",
                nameof(documentNumber));

        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new ArgumentException(
                "Invoice Number is required.",
                nameof(invoiceNumber));

        if (supplierId == Guid.Empty)
            throw new ArgumentException(
                "Supplier is required.",
                nameof(supplierId));

        if (warehouseId == Guid.Empty)
            throw new ArgumentException(
                "Warehouse is required.",
                nameof(warehouseId));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException(
                "Currency is required.",
                nameof(currency));

        if (exchangeRate <= 0)
            throw new ArgumentException(
                "Exchange Rate must be greater than zero.",
                nameof(exchangeRate));

        if (documentDate > DateTime.UtcNow.AddDays(1))
            throw new ArgumentException(
                "Document Date cannot be in the future.",
                nameof(documentDate));

        if (invoiceDate > DateTime.UtcNow.AddDays(1))
            throw new ArgumentException(
                "Invoice Date cannot be in the future.",
                nameof(invoiceDate));

        DocumentNumber = documentNumber.Trim();

        DocumentDate = documentDate;

        AssignOrganization(
            fiscalYearId,
            companyId,
            branchId);

        Notes = notes;

        InvoiceNumber = invoiceNumber.Trim();

        InvoiceDate = invoiceDate;

        SupplierId = supplierId;

        WarehouseId = warehouseId;

        Currency = currency.Trim().ToUpperInvariant();

        ExchangeRate = exchangeRate;

        Status = DocumentStatus.Draft;
    }

    public void AddLine(
        Guid productId,
        decimal quantity,
        decimal unitCost,
        decimal discountAmount,
        decimal taxAmount,
        string batchNumber,
        DateTime? expiryDate,
        string? notes)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Cannot modify a posted purchase.");

        var line = new PurchaseLine(
            productId,
            quantity,
            unitCost,
            discountAmount,
            taxAmount,
            batchNumber,
            expiryDate,
            notes);

        _lines.Add(line);

        RecalculateTotals();
    }

    public void RemoveLine(Guid lineId)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Cannot modify a posted purchase.");

        var line = _lines.FirstOrDefault(x => x.Id == lineId);

        if (line is null)
            throw new InvalidOperationException(
                "Purchase line not found.");

        _lines.Remove(line);

        RecalculateTotals();
    }

    public void RecalculateTotals()
    {
        TotalBeforeDiscount =
            _lines.Sum(x => x.Quantity * x.UnitCost);

        DiscountAmount =
            _lines.Sum(x => x.DiscountAmount);

        TaxAmount =
            _lines.Sum(x => x.TaxAmount);

        NetTotal =
            TotalBeforeDiscount
            - DiscountAmount
            + TaxAmount;
    }

    public override void Post()
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Only draft purchases can be posted.");

        if (_lines.Count == 0)
            throw new InvalidOperationException(
                "Purchase document has no lines.");

        RecalculateTotals();

        if (NetTotal <= 0)
            throw new InvalidOperationException(
                "Purchase total must be greater than zero.");

        Status = DocumentStatus.Posted;
    }

    public override void Cancel()
    {
        base.Cancel();
    }
}