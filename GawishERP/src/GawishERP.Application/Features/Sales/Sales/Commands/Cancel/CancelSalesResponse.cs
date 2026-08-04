namespace GawishERP.Application.Features.Sales.Sales.Commands.Cancel;

public sealed class CancelSalesResponse
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}