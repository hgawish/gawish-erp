using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Security;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Authentication;
using GawishERP.Infrastructure.Persistence;
using GawishERP.Infrastructure.Repositories;
using GawishERP.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GawishERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        #region Database

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        #endregion

        #region HttpContext

        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();

        #endregion

        #region Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IProductRepository, ProductRepository>();

        #endregion

        #region Authentication

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        #endregion

        return services;
    }
}