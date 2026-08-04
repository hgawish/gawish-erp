namespace GawishERP.Application.Features.Purchasing.PurchaseReturn.Commands.Post;

public sealed class PostPurchaseReturnResponse
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}