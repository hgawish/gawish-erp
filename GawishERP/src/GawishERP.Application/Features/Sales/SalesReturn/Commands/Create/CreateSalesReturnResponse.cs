namespace GawishERP.Application.Features.Sales.SalesReturn.Commands.Create;

public sealed class CreateSalesReturnResponse
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;
}