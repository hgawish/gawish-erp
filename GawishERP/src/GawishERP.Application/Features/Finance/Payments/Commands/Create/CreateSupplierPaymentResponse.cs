namespace GawishERP.Application.Features.Finance.Payments.Commands.Create;

public sealed class CreateSupplierPaymentResponse
{
    public Guid Id { get; init; }
    public string DocumentNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}
