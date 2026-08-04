using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // ============================================================
    // Security
    // ============================================================

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // ============================================================
    // Master Data
    // ============================================================

    public DbSet<Product> Products => Set<Product>();

    public DbSet<Customer> Customers => Set<Customer>();

    public DbSet<Supplier> Suppliers => Set<Supplier>();

    public DbSet<Warehouse> Warehouses => Set<Warehouse>();

    // ============================================================
    // Accounting
    // ============================================================

    public DbSet<Account> Accounts => Set<Account>();

    public DbSet<JournalEntryHeader> JournalEntryHeaders => Set<JournalEntryHeader>();

    public DbSet<JournalEntryLine> JournalEntryLines => Set<JournalEntryLine>();
    public DbSet<LedgerTransaction> LedgerTransactions => Set<LedgerTransaction>();

    public DbSet<AccountBalance> AccountBalances => Set<AccountBalance>();
    public DbSet<FinancialStatementNode> AccountReportCategories
   => Set<FinancialStatementNode>();

    // ============================================================
    // Inventory
    // ============================================================

    public DbSet<StockTransaction> StockTransactions => Set<StockTransaction>();

    public DbSet<InventoryBalance> InventoryBalances => Set<InventoryBalance>();

    // ============================================================
    // Opening Balance
    // ============================================================

    public DbSet<OpeningBalanceHeader> OpeningBalanceHeaders => Set<OpeningBalanceHeader>();

    public DbSet<OpeningBalanceLine> OpeningBalanceLines => Set<OpeningBalanceLine>();

    // ============================================================
    // Purchasing
    // ============================================================

    public DbSet<PurchaseHeader> PurchaseHeaders => Set<PurchaseHeader>();

    public DbSet<PurchaseLine> PurchaseLines => Set<PurchaseLine>();

    public DbSet<PurchaseReturnHeader> PurchaseReturnHeaders => Set<PurchaseReturnHeader>();

    public DbSet<PurchaseReturnLine> PurchaseReturnLines => Set<PurchaseReturnLine>();

    // ============================================================
    // Sales
    // ============================================================

    public DbSet<SalesHeader> SalesHeaders => Set<SalesHeader>();

    public DbSet<SalesLine> SalesLines => Set<SalesLine>();

    public DbSet<SalesReturnHeader> SalesReturnHeaders => Set<SalesReturnHeader>();

    public DbSet<SalesReturnLine> SalesReturnLines => Set<SalesReturnLine>();

    // ============================================================
    // System
    // ============================================================

    public DbSet<FiscalYear> FiscalYears => Set<FiscalYear>();

    public DbSet<NumberSeries> NumberSeries => Set<NumberSeries>();

    // ============================================================
    // Model Configuration
    // ============================================================

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
   
}