namespace GawishERP.Application.Features.Purchasing.Purchase.Commands.Create;

public sealed record CreatePurchaseResponse
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;
}