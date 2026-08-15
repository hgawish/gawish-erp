namespace GawishERP.Application.Features.Sales.SalesDeliveries.Commands.CancelSalesDelivery;

public sealed class CancelSalesDeliveryResponse
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}
