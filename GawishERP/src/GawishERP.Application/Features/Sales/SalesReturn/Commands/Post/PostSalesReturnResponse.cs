namespace GawishERP.Application.Features.Sales.SalesReturn.Commands.Post;

public sealed class PostSalesReturnResponse
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}