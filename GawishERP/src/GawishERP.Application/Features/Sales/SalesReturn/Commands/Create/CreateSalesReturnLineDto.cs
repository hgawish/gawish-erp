namespace GawishERP.Application.Features.Sales.SalesReturn.Commands.Create;

public sealed class CreateSalesReturnLineDto
{
    public Guid SalesLineId { get; init; }

    public Guid ProductId { get; init; }

    public decimal Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public string? Notes { get; init; }
}