namespace GawishERP.Application.Features.Sales.Sales.Commands.Create;

public sealed class CreateSalesResponse
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;
}