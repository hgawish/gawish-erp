using GawishERP.Domain.Common;

namespace GawishERP.Domain.Entities;

public class OpeningBalanceHeader : BaseDocumentEntity
{
    private readonly List<OpeningBalanceLine> _lines = new();

    public Guid WarehouseId { get; private set; }

    // Navigation

    public Warehouse Warehouse { get; private set; } = null!;

    public IReadOnlyCollection<OpeningBalanceLine> Lines => _lines.AsReadOnly();

    private OpeningBalanceHeader()
    {
    }

    public OpeningBalanceHeader(
        string documentNumber,
        Guid warehouseId,
        DateTime documentDate,
        string? notes)
    {
        if (string.IsNullOrWhiteSpace(documentNumber))
            throw new ArgumentException(
                "Document number is required.",
                nameof(documentNumber));

        DocumentNumber = documentNumber.Trim();
        DocumentDate = documentDate;
        WarehouseId = warehouseId;
        Notes = notes?.Trim();

        Status = DocumentStatus.Draft;
    }

    public void AddLine(
        Guid productId,
        decimal quantity,
        decimal unitCost,
        string? notes)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Cannot modify a posted opening balance.");

        var line = new OpeningBalanceLine(
            Id,
            productId,
            quantity,
            unitCost,
            notes);

        _lines.Add(line);
    }

    public void RemoveLine(Guid lineId)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Cannot modify a posted opening balance.");

        var line = _lines.FirstOrDefault(x => x.Id == lineId);

        if (line is null)
            throw new InvalidOperationException(
                "Opening balance line not found.");

        _lines.Remove(line);
    }

    public override void Post()
    {
        if (_lines.Count == 0)
            throw new InvalidOperationException(
                "Opening balance document contains no lines.");

        base.Post();
    }

    public override void Cancel()
    {
        base.Cancel();
    }

    public void UpdateNotes(string? notes)
    {
        if (Status != DocumentStatus.Draft)
            throw new InvalidOperationException(
                "Cannot modify a posted opening balance.");

        Notes = notes?.Trim();
    }
}