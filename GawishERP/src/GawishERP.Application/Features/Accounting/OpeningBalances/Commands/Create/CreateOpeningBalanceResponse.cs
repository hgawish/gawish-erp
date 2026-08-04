namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Create;

public sealed class CreateOpeningBalanceResponse
{
    public Guid Id { get; init; }

    public string Message { get; init; } =
        "Opening Balance created successfully.";
}