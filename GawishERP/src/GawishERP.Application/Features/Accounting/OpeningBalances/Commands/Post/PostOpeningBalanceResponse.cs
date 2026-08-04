namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Post;

public sealed class PostOpeningBalanceResponse
{
    public Guid Id { get; init; }

    public string Message { get; init; } =
        "Opening Balance posted successfully.";
}