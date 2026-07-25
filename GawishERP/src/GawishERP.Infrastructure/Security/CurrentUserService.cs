using System.Security.Claims;
using GawishERP.Application.Common.Security;
using Microsoft.AspNetCore.Http;

namespace GawishERP.Infrastructure.Security;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var id = _httpContextAccessor.HttpContext?
                .User?
                .FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(id, out var guid))
                return guid;

            return null;
        }
    }

    public string? Email =>
        _httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.Email);

    public string? FirstName =>
        _httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.GivenName);

    public string? LastName =>
        _httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.Surname);

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public IReadOnlyCollection<string> Permissions =>
        _httpContextAccessor.HttpContext?
            .User?
            .FindAll("permission")
            .Select(x => x.Value)
            .ToList()
        ?? new List<string>();
}