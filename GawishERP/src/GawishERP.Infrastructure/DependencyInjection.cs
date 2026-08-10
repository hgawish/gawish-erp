using GawishERP.Application.Common.Interfaces;
using GawishERP.Application.Common.Interfaces.Repositories;
using GawishERP.Application.Common.Posting;
using GawishERP.Application.Common.Security;
using GawishERP.Application.Features.Sales.SalesOrders;
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

        services.AddScoped<ILedgerTransactionRepository, LedgerTransactionRepository>();

        services.AddScoped<IAccountBalanceRepository, AccountBalanceRepository>();

        services.AddScoped<IFinancialStatementNodeRepository, FinancialStatementNodeRepository>();

        //=========================================================
        // Sales Quotations
        //=========================================================

        services.AddScoped<ISalesQuotationRepository, SalesQuotationRepository>();

        //=========================================================
        // Sales Orders
        //=========================================================

        services.AddScoped<ISalesOrderRepository, SalesOrderRepository>();

        #endregion

        #region Domain Services

        services.AddScoped<IInventoryService, InventoryService>();

        services.AddScoped<IDocumentNumberService, DocumentNumberService>();

        services.AddScoped<IPostingEngine, PostingEngine>();

        services.AddScoped<IAccountResolver, AccountResolver>();

        services.AddScoped<IPurchasePostingService, PurchasePostingService>();

        services.AddScoped<IPurchaseReturnPostingService, PurchaseReturnPostingService>();

        services.AddScoped<ISalesPostingService, SalesPostingService>();

        services.AddScoped<ISalesReturnPostingService, SalesReturnPostingService>();

        services.AddScoped<ILedgerPostingService, LedgerPostingService>();

        services.AddScoped<IFinancialReportingService, FinancialReportingService>();

        services.AddScoped<IPostingProfileRepository, PostingProfileRepository>();

        #endregion

        #region Authentication

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        #endregion

        #region Financial Reporting

        services.AddScoped<IFinancialFormulaEvaluator, FinancialFormulaEvaluator>();

        services.AddScoped<IFinancialStatementCalculator, FinancialStatementCalculator>();

        #endregion

        #region AutoMapper

        services.AddAutoMapper(
            _ => { },
            typeof(SalesOrderMappingProfile).Assembly);
        services.AddScoped<ISalesDeliveryRepository, SalesDeliveryRepository>();

        #endregion

        return services;
    }
}