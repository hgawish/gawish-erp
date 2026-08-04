namespace GawishERP.Application.Features.Sales.SalesReturn.Queries.GetById;

public sealed class SalesReturnLineDto
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ProductCode { get; set; } = string.Empty;

    public string ProductName { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }

    public string? Notes { get; set; }
}