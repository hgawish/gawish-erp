namespace GawishERP.Application.Common.Security;

public interface ICurrentUserService
{
    Guid? UserId { get; }

    string? Email { get; }

    string? FirstName { get; }

    string? LastName { get; }

    bool IsAuthenticated { get; }

    IReadOnlyCollection<string> Permissions { get; }
}