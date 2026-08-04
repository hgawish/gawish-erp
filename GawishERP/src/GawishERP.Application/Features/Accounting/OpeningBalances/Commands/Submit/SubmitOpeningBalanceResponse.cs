namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Submit;

public sealed class SubmitOpeningBalanceResponse
{
    public Guid Id { get; init; }

    public string Message { get; init; } =
        "Opening Balance submitted successfully.";
}