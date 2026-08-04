namespace GawishERP.Application.Features.Sales.Sales.Commands.Create;

public sealed class CreateSalesLineDto
{
    public Guid ProductId { get; init; }

    public decimal Quantity { get; init; }

    public decimal UnitPrice { get; init; }

    public decimal DiscountAmount { get; init; }

    public decimal TaxAmount { get; init; }

    public string? BatchNumber { get; init; }

    public DateTime? ExpiryDate { get; init; }

    public string? Notes { get; init; }
}