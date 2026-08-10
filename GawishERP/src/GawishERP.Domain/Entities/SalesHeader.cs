using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class SalesHeader : BaseDocumentEntity
{
    private readonly List<SalesLine> _lines = new();

    public Guid CustomerId { get; private set; }

    public Guid WarehouseId { get; private set; }

    public string Currency { get; private set; } = "EGP";

    public decimal ExchangeRate { get; private set; } = 1;

    public decimal TotalBeforeDiscount { get; private set; }

    public decimal DiscountAmount { get; private set; }

    public decimal TaxAmount { get; private set; }

    public decimal NetTotal { get; private set; }

    //=========================================================
    // Navigation
    //=========================================================

    public Customer Customer { get; private set; } = null!;

    public Warehouse Warehouse { get; private set; } = null!;

    public IReadOnlyCollection<SalesLine> Lines =>
        _lines.AsReadOnly();

    private SalesHeader()
    {
    }

    public SalesHeader(
        string documentNumber,
        DateTime documentDate,

        Guid fiscalYearId,
        Guid? companyId,
        Guid? branchId,

        Guid customerId,
        Guid warehouseId,

        string currency,
        decimal exchangeRate,

        string? notes)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new ArgumentException(nameof(documentNumber));

        if (customerId == Guid.Empty)
            throw new ArgumentException(nameof(customerId));

        if (warehouseId == Guid.Empty)
            throw new ArgumentException(nameof(warehouseId));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException(nameof(currency));

        if (exchangeRate <= 0)
            throw new ArgumentException(nameof(exchangeRate));

        DocumentNumber = documentNumber.Trim();

        DocumentDate = documentDate;

        AssignOrganization(
            fiscalYearId,
            companyId,
            branchId);

        CustomerId = customerId;

        WarehouseId = warehouseId;

        Currency = currency.Trim().ToUpperInvariant();

        ExchangeRate = exchangeRate;

        Notes = notes;

        Status = DocumentStatus.Draft;
    }

    public void AddLine(
        Guid productId,
        decimal quantity,
        decimal unitPrice,
        decimal discount,
        decimal tax,
        string? batchNumber,
        DateTime? expiryDate,
        string? notes)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Cannot modify posted document.");

        var line = new SalesLine(
            productId,
            quantity,
            unitPrice,
            discount,
            tax,
            batchNumber,
            expiryDate,
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
        TotalBeforeDiscount =
            _lines.Sum(x => x.Quantity * x.UnitPrice);

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
        if (!_lines.Any())
            throw new InvalidOperationException(
                "Document has no lines.");

        Recalculate();

        if (NetTotal <= 0)
            throw new InvalidOperationException(
                "Sales total must be greater than zero.");

        base.Post();
    }

    public override void Cancel()
    {
        base.Cancel();
    }
}