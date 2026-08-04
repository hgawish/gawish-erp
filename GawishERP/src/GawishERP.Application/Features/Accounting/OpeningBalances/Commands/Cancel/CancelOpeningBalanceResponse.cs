namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Cancel;

public sealed class CancelOpeningBalanceResponse
{
    public Guid Id { get; init; }

    public string Message { get; init; } =
        "Opening Balance cancelled successfully.";
}