namespace GawishERP.Application.Features.Accounting.Accounts.Commands.Deactivate;

public sealed class DeactivateAccountResponse
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}