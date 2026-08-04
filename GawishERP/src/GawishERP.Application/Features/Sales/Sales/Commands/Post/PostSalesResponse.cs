namespace GawishERP.Application.Features.Sales.Sales.Commands.Post;

public sealed class PostSalesResponse
{
    public Guid Id { get; init; }

    public string DocumentNumber { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}