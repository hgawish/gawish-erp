namespace GawishERP.Application.Features.Sales.Sales.Queries.GetList;

public sealed class SalesListItemDto
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public DateTime DocumentDate { get; init; }

    public string CustomerName { get; init; } = string.Empty;

    public string WarehouseName { get; init; } = string.Empty;

    public decimal NetTotal { get; init; }

    public string Currency { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;
}