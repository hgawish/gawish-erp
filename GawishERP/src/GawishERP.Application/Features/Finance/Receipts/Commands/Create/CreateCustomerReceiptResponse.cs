namespace GawishERP.Application.Features.Finance.Receipts.Commands.Create;

public sealed class CreateCustomerReceiptResponse
{
    public Guid Id { get; init; }
    public string DocumentNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
