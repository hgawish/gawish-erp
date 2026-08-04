namespace GawishERP.Application.Features.Accounting.Accounts.Commands.Create;

public sealed class CreateAccountResponse
{
    public Guid Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}