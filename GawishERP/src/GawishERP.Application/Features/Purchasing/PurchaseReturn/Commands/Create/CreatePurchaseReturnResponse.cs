namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Commands.Create;

public sealed class CreatePurchaseReturnResponse
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;
}