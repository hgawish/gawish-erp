namespace GawishERP.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(
        Guid userId,
        string firstName,
        string lastName,
        string email,
        IEnumerable<string> permissions);
}