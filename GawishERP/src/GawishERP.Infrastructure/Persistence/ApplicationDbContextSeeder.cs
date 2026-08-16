using BCrypt.Net;
using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence;

public static class ApplicationDbContextSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();
        await SeedRolesAsync(context);
        await SeedPermissionsAsync(context);
        await SeedSuperAdminRolePermissionsAsync(context);
        await SeedAdminUserAsync(context);
        await SeedNumberSeriesAsync(context);
        await SeedPostingProfilesAsync(context);
    }

    private static async Task SeedRolesAsync(ApplicationDbContext context)
    {
        var roles = new List<Role>
        {
            new("SuperAdmin", "System Administrator"),
            new("Admin", "Administrator"),
            new("Sales", "Sales Department"),
            new("Warehouse", "Warehouse Department"),
            new("Accountant", "Accounting Department")
        };

        foreach (var role in roles)
        {
            if (!await context.Roles.AnyAsync(x => x.Name == role.Name))
                context.Roles.Add(role);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedPermissionsAsync(ApplicationDbContext context)
    {
        var permissions = new List<Permission>
        {
            new("Warehouses.View", "View Warehouses"),
            new("Warehouses.Create", "Create Warehouses"),
            new("Warehouses.Edit", "Edit Warehouses"),
            new("Warehouses.Delete", "Delete Warehouses"),
            new("Users.View", "View Users"),
            new("Users.Create", "Create Users"),
            new("Users.Update", "Update Users"),
            new("Users.Delete", "Delete Users"),
            new("Roles.View", "View Roles"),
            new("Roles.Manage", "Manage Roles"),
            new("Products.View", "View Products"),
            new("Products.Create", "Create Products"),
            new("Products.Update", "Update Products"),
            new("Products.Delete", "Delete Products"),
            new("Customers.View", "View Customers"),
            new("Customers.Create", "Create Customers"),
            new("Customers.Update", "Update Customers"),
            new("Customers.Delete", "Delete Customers"),
            new("Suppliers.View", "View Suppliers"),
            new("Suppliers.Create", "Create Suppliers"),
            new("Suppliers.Edit", "Edit Suppliers"),
            new("Suppliers.Delete", "Delete Suppliers"),
            new("Sales.View", "View Sales"),
            new("Sales.Create", "Create Sales"),
            new("Sales.Update", "Update Sales"),
            new("Purchases.View", "View Purchases"),
            new("Purchases.Create", "Create Purchases"),
            new("Inventory.View", "View Inventory"),
            new("Inventory.Update", "Update Inventory")
        };

        foreach (var permission in permissions)
        {
            if (!await context.Permissions.AnyAsync(x => x.Name == permission.Name))
                context.Permissions.Add(permission);
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedSuperAdminRolePermissionsAsync(ApplicationDbContext context)
    {
        var superAdminRole = await context.Roles.SingleAsync(x => x.Name == "SuperAdmin");
        var allPermissions = await context.Permissions.ToListAsync();

        foreach (var permission in allPermissions)
        {
            var exists = await context.RolePermissions.AnyAsync(x =>
                x.RoleId == superAdminRole.Id && x.PermissionId == permission.Id);

            if (!exists)
                context.RolePermissions.Add(new RolePermission(superAdminRole.Id, permission.Id));
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedAdminUserAsync(ApplicationDbContext context)
    {
        var adminUser = await context.Users.SingleOrDefaultAsync(
            x => x.Email == "admin@gawisherp.com");

        if (adminUser == null)
        {
            adminUser = new User(
                "System",
                "Administrator",
                "admin@gawisherp.com",
                BCrypt.Net.BCrypt.HashPassword("Admin@123"));

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }

        var superAdminRole = await context.Roles.SingleAsync(x => x.Name == "SuperAdmin");

        var hasRole = await context.UserRoles.AnyAsync(x =>
            x.UserId == adminUser.Id && x.RoleId == superAdminRole.Id);

        if (!hasRole)
        {
            context.UserRoles.Add(new UserRole(adminUser.Id, superAdminRole.Id));
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedNumberSeriesAsync(ApplicationDbContext context)
    {
        Console.WriteLine("Checking Number Series...");

        var defaultSeries = new List<NumberSeries>
        {
            new(DocumentType.OpeningBalance, "OB"),
            new(DocumentType.Purchase, "PO"),
            new(DocumentType.PurchaseReturn, "PR"),
            new(DocumentType.Sales, "SO"),
            new(DocumentType.SalesReturn, "SR"),
            new(DocumentType.Transfer, "TR"),
            new(DocumentType.Adjustment, "ADJ"),
            new(DocumentType.Production, "MO"),
            new(DocumentType.StockCount, "SC")
        };

        foreach (var series in defaultSeries)
        {
            var exists = await context.NumberSeries.AnyAsync(
                x => x.DocumentType == series.DocumentType);

            if (!exists)
                context.NumberSeries.Add(series);
        }

        await context.SaveChangesAsync();
        Console.WriteLine("Number Series check completed successfully.");
    }

    // ======================================================
    // Posting Profiles
    // ======================================================
    // IMPORTANT:
    // System posting profiles are resolved by Account.Code, never by
    // hard-coded database GUIDs. This keeps the seed portable across
    // databases and prevents broken profiles after a database recreation.
    // ======================================================

    private static async Task SeedPostingProfilesAsync(ApplicationDbContext context)
    {
        Console.WriteLine("Checking Posting Profiles...");

        async Task<Guid> ResolveAccountIdAsync(string code, string name)
        {
            var account = await context.Accounts
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Code == code);

            if (account is null)
            {
                throw new InvalidOperationException(
                    $"Required system account '{code} - {name}' was not found.");
            }

            if (!account.IsPostingAccount)
            {
                throw new InvalidOperationException(
                    $"Required system account '{code} - {name}' does not allow posting.");
            }

            return account.Id;
        }

        // Current chart of accounts used by GawishERP.
        var inventoryAccountId = await ResolveAccountIdAsync("1140", "Inventory");
        var customerAccountId = await ResolveAccountIdAsync("1130", "Customers");
        var supplierAccountId = await ResolveAccountIdAsync("2110", "Suppliers");
        var salesAccountId = await ResolveAccountIdAsync("4100", "Sales Revenue");
        var salesReturnsAccountId = await ResolveAccountIdAsync("4200", "Sales Returns");
        var costOfSalesAccountId = await ResolveAccountIdAsync("5100", "Cost Of Sales");

        static void ResetLines(
            PostingProfile profile,
            params PostingProfileLine[] lines)
        {
            profile.ClearLines();

            foreach (var line in lines)
                profile.AddLine(line);
        }

        // ==================================================
        // Purchase
        // Dr Inventory / Cr Suppliers
        // ==================================================
        var purchaseProfile = await context.PostingProfiles
            .Include(x => x.Lines)
            .SingleOrDefaultAsync(x =>
                x.Code == "PURCHASE_DEFAULT" &&
                x.DocumentType == DocumentType.Purchase);

        if (purchaseProfile is null)
        {
            purchaseProfile = new PostingProfile(
                "PURCHASE_DEFAULT",
                "Purchase - Default",
                DocumentType.Purchase,
                inventoryAccountId,
                supplierAccountId,
                CashFlowCategory.None);

            context.PostingProfiles.Add(purchaseProfile);
        }
        else
        {
            purchaseProfile.Update(
                "Purchase - Default",
                inventoryAccountId,
                supplierAccountId,
                CashFlowCategory.None);
        }

        ResetLines(
            purchaseProfile,
            new PostingProfileLine(
                1, PostingEntryType.Debit, inventoryAccountId,
                PostingAmountSource.NetTotal, 100m, "Purchase - Inventory"),
            new PostingProfileLine(
                2, PostingEntryType.Credit, supplierAccountId,
                PostingAmountSource.NetTotal, 100m, "Purchase - Supplier"));

        // ==================================================
        // Purchase Return
        // Dr Suppliers / Cr Inventory
        // ==================================================
        var purchaseReturnProfile = await context.PostingProfiles
            .Include(x => x.Lines)
            .SingleOrDefaultAsync(x =>
                x.Code == "PURCHASE_RETURN_DEFAULT" &&
                x.DocumentType == DocumentType.PurchaseReturn);

        if (purchaseReturnProfile is null)
        {
            purchaseReturnProfile = new PostingProfile(
                "PURCHASE_RETURN_DEFAULT",
                "Purchase Return - Default",
                DocumentType.PurchaseReturn,
                supplierAccountId,
                inventoryAccountId,
                CashFlowCategory.None);

            context.PostingProfiles.Add(purchaseReturnProfile);
        }
        else
        {
            purchaseReturnProfile.Update(
                "Purchase Return - Default",
                supplierAccountId,
                inventoryAccountId,
                CashFlowCategory.None);
        }

        ResetLines(
            purchaseReturnProfile,
            new PostingProfileLine(
                1, PostingEntryType.Debit, supplierAccountId,
                PostingAmountSource.NetTotal, 100m, "Purchase Return - Supplier"),
            new PostingProfileLine(
                2, PostingEntryType.Credit, inventoryAccountId,
                PostingAmountSource.NetTotal, 100m, "Purchase Return - Inventory"));

        // ==================================================
        // Sales
        // Dr Customers / Cr Sales
        // Dr Cost Of Sales / Cr Inventory
        // ==================================================
        var salesProfile = await context.PostingProfiles
            .Include(x => x.Lines)
            .SingleOrDefaultAsync(x =>
                x.Code == "SALES_DEFAULT" &&
                x.DocumentType == DocumentType.Sales);

        if (salesProfile is null)
        {
            salesProfile = new PostingProfile(
                "SALES_DEFAULT",
                "Sales - Default",
                DocumentType.Sales,
                customerAccountId,
                salesAccountId,
                CashFlowCategory.None);

            context.PostingProfiles.Add(salesProfile);
        }
        else
        {
            salesProfile.Update(
                "Sales - Default",
                customerAccountId,
                salesAccountId,
                CashFlowCategory.None);
        }

        ResetLines(
            salesProfile,
            new PostingProfileLine(
                1, PostingEntryType.Debit, customerAccountId,
                PostingAmountSource.NetTotal, 100m, "Sales - Customer"),
            new PostingProfileLine(
                2, PostingEntryType.Credit, salesAccountId,
                PostingAmountSource.NetTotal, 100m, "Sales - Revenue"),
            new PostingProfileLine(
                3, PostingEntryType.Debit, costOfSalesAccountId,
                PostingAmountSource.Cost, 100m, "Sales - COGS"),
            new PostingProfileLine(
                4, PostingEntryType.Credit, inventoryAccountId,
                PostingAmountSource.Cost, 100m, "Sales - Inventory"));

        // ==================================================
        // Sales Return
        // Dr Sales Returns / Cr Customers
        // Dr Inventory / Cr Cost Of Sales
        // ==================================================
        var salesReturnProfile = await context.PostingProfiles
            .Include(x => x.Lines)
            .SingleOrDefaultAsync(x =>
                x.Code == "SALES_RETURN_DEFAULT" &&
                x.DocumentType == DocumentType.SalesReturn);

        if (salesReturnProfile is null)
        {
            salesReturnProfile = new PostingProfile(
                "SALES_RETURN_DEFAULT",
                "Sales Return - Default",
                DocumentType.SalesReturn,
                salesReturnsAccountId,
                customerAccountId,
                CashFlowCategory.None);

            context.PostingProfiles.Add(salesReturnProfile);
        }
        else
        {
            salesReturnProfile.Update(
                "Sales Return - Default",
                salesReturnsAccountId,
                customerAccountId,
                CashFlowCategory.None);
        }

        ResetLines(
            salesReturnProfile,
            new PostingProfileLine(
                1, PostingEntryType.Debit, salesReturnsAccountId,
                PostingAmountSource.NetTotal, 100m, "Sales Return - Revenue"),
            new PostingProfileLine(
                2, PostingEntryType.Credit, customerAccountId,
                PostingAmountSource.NetTotal, 100m, "Sales Return - Customer"),
            new PostingProfileLine(
                3, PostingEntryType.Debit, inventoryAccountId,
                PostingAmountSource.Cost, 100m, "Sales Return - Inventory"),
            new PostingProfileLine(
                4, PostingEntryType.Credit, costOfSalesAccountId,
                PostingAmountSource.Cost, 100m, "Sales Return - COGS"));

        await context.SaveChangesAsync();

        Console.WriteLine("Posting Profiles seeded successfully.");
    }
}
