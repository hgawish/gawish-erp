namespace GawishERP.Application.Features.Purchasing.Purchase.Commands.Post;

public sealed record PostPurchaseResponse
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}