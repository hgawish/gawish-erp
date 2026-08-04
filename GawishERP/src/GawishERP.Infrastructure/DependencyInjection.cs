using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Security;
using GawishERP.Domain.Interfaces;
using GawishERP.Infrastructure.Authentication;
using GawishERP.Infrastructure.Persistence;
using GawishERP.Infrastructure.Persistence.Repositories;
using GawishERP.Infrastructure.Persistence.UnitOfWork;
using GawishERP.Infrastructure.Security;
using GawishERP.Infrastructure.Services;
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

        #region Unit Of Work

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        #endregion

        #region Repositories

        services.AddScoped<IAccountRepository, AccountRepository>();

        services.AddScoped<ICustomerRepository, CustomerRepository>();

        services.AddScoped<IFiscalYearRepository, FiscalYearRepository>();

        services.AddScoped<IInventoryBalanceRepository, InventoryBalanceRepository>();

        services.AddScoped<IJournalEntryRepository, JournalEntryRepository>();

        services.AddScoped<INumberSeriesRepository, NumberSeriesRepository>();

        services.AddScoped<IOpeningBalanceRepository, OpeningBalanceRepository>();

        services.AddScoped<IProductRepository, ProductRepository>();

        services.AddScoped<IPurchaseRepository, PurchaseRepository>();

        services.AddScoped<IPurchaseReturnRepository, PurchaseReturnRepository>();

        services.AddScoped<ISalesRepository, SalesRepository>();

        services.AddScoped<ISalesReturnRepository, SalesReturnRepository>();

        services.AddScoped<IStockTransactionRepository, StockTransactionRepository>();

        services.AddScoped<ISupplierRepository, SupplierRepository>();

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IWarehouseRepository, WarehouseRepository>();

        #endregion

        #region Domain Services

        services.AddScoped<IInventoryService, InventoryService>();

        services.AddScoped<IDocumentNumberService, DocumentNumberService>();

        #endregion

        #region Authentication

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<ILedgerTransactionRepository, LedgerTransactionRepository>();

        services.AddScoped<IAccountBalanceRepository, AccountBalanceRepository>();
        services.AddScoped<ILedgerPostingService, LedgerPostingService>();
        services.AddScoped<IFinancialStatementNodeRepository,FinancialStatementNodeRepository>();
        #endregion

        return services;
    }
}