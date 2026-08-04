namespace GawishERP.Application.Features.Accounting.OpeningBalances.Commands.Approve;

public sealed class ApproveOpeningBalanceResponse
{
    public Guid Id { get; init; }

    public string Message { get; init; } =
        "Opening Balance approved successfully.";
}