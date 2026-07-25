namespace GawishERP.Application.Features.Authentication.Commands.Login;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }
}