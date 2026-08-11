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

    // ======================================================
    // Roles
    // ======================================================

    private static async Task SeedRolesAsync(
        ApplicationDbContext context)
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
            if (!await context.Roles.AnyAsync(
                    x => x.Name == role.Name))
            {
                context.Roles.Add(role);
            }
        }

        await context.SaveChangesAsync();
    }

    // ======================================================
    // Permissions
    // ======================================================

    private static async Task SeedPermissionsAsync(
        ApplicationDbContext context)
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
            if (!await context.Permissions.AnyAsync(
                    x => x.Name == permission.Name))
            {
                context.Permissions.Add(permission);
            }
        }

        await context.SaveChangesAsync();
    }

    // ======================================================
    // SuperAdmin Permissions
    // ======================================================

    private static async Task SeedSuperAdminRolePermissionsAsync(
        ApplicationDbContext context)
    {
        var superAdminRole =
            await context.Roles.SingleAsync(
                x => x.Name == "SuperAdmin");

        var allPermissions =
            await context.Permissions.ToListAsync();

        foreach (var permission in allPermissions)
        {
            bool exists =
                await context.RolePermissions.AnyAsync(x =>
                    x.RoleId == superAdminRole.Id &&
                    x.PermissionId == permission.Id);

            if (!exists)
            {
                context.RolePermissions.Add(
                    new RolePermission(
                        superAdminRole.Id,
                        permission.Id));
            }
        }

        await context.SaveChangesAsync();
    }

    // ======================================================
    // Admin User
    // ======================================================

    private static async Task SeedAdminUserAsync(
        ApplicationDbContext context)
    {
        var adminUser =
            await context.Users.SingleOrDefaultAsync(
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

        var superAdminRole =
            await context.Roles.SingleAsync(
                x => x.Name == "SuperAdmin");

        bool hasRole =
            await context.UserRoles.AnyAsync(x =>
                x.UserId == adminUser.Id &&
                x.RoleId == superAdminRole.Id);

        if (!hasRole)
        {
            context.UserRoles.Add(
                new UserRole(
                    adminUser.Id,
                    superAdminRole.Id));

            await context.SaveChangesAsync();
        }
    }

    // ======================================================
    // Number Series
    // ======================================================

    private static async Task SeedNumberSeriesAsync(
        ApplicationDbContext context)
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
            bool exists =
                await context.NumberSeries.AnyAsync(
                    x => x.DocumentType == series.DocumentType);

            if (!exists)
            {
                context.NumberSeries.Add(series);

                Console.WriteLine(
                    $"Added Number Series: {series.DocumentType}");
            }
            else
            {
                Console.WriteLine(
                    $"Number Series already exists: {series.DocumentType}");
            }
        }

        await context.SaveChangesAsync();

        Console.WriteLine(
            "Number Series check completed successfully.");
    }

    // ======================================================
    // Posting Profiles
    // ======================================================

    private static async Task SeedPostingProfilesAsync(
        ApplicationDbContext context)
    {
        Console.WriteLine("Checking Posting Profiles...");

        // ==================================================
        // Real account IDs from the current database
        // ==================================================

        var inventoryAccountId =
            Guid.Parse(
                "57D2B2AA-A621-430E-818E-7BDDB443C443");

        var supplierAccountId =
            Guid.Parse(
                "8B49E22F-A123-4873-AB5B-AF8FE3DC607D");

        // ==================================================
        // Validate Accounts
        // ==================================================

        bool inventoryExists =
            await context.Accounts.AnyAsync(
                x => x.Id == inventoryAccountId);

        if (!inventoryExists)
        {
            throw new InvalidOperationException(
                "Inventory account 1140 was not found.");
        }

        bool supplierExists =
            await context.Accounts.AnyAsync(
                x => x.Id == supplierAccountId);

        if (!supplierExists)
        {
            throw new InvalidOperationException(
                "Supplier account 2110 was not found.");
        }

        // ==================================================
        // Purchase
        //
        // Debit  : Inventory 1140
        // Credit : Suppliers 2110
        // ==================================================

        var purchaseProfile =
            await context.PostingProfiles
                .Include(x => x.Lines)
                .SingleOrDefaultAsync(
                    x =>
                        x.Code == "PURCHASE_DEFAULT" &&
                        x.DocumentType == DocumentType.Purchase);

        if (purchaseProfile == null)
        {
            purchaseProfile = new PostingProfile(
                code: "PURCHASE_DEFAULT",
                name: "Purchase - Default",
                documentType: DocumentType.Purchase,
                debitAccountId: inventoryAccountId,
                creditAccountId: supplierAccountId,
                cashFlowCategory: CashFlowCategory.None);

            context.PostingProfiles.Add(purchaseProfile);

            Console.WriteLine(
                "Added Posting Profile: PURCHASE_DEFAULT");
        }
        else
        {
            purchaseProfile.Update(
                name: "Purchase - Default",
                debitAccountId: inventoryAccountId,
                creditAccountId: supplierAccountId,
                cashFlowCategory: CashFlowCategory.None);

            Console.WriteLine(
                "Posting Profile already exists: PURCHASE_DEFAULT");
        }

        // ==================================================
        // Purchase Lines
        // ==================================================

        if (!purchaseProfile.Lines.Any())
        {
            purchaseProfile.AddLine(
                new PostingProfileLine(
                    sequence: 1,
                    entryType: PostingEntryType.Debit,
                    accountId: inventoryAccountId,
                    amountSource: PostingAmountSource.NetTotal,
                    percentage: 100m,
                    description: "Purchase - Inventory"));

            purchaseProfile.AddLine(
                new PostingProfileLine(
                    sequence: 2,
                    entryType: PostingEntryType.Credit,
                    accountId: supplierAccountId,
                    amountSource: PostingAmountSource.NetTotal,
                    percentage: 100m,
                    description: "Purchase - Supplier"));

            Console.WriteLine(
                "Added Purchase Posting Profile Lines.");
        }

        // ==================================================
        // Purchase Return
        //
        // Debit  : Suppliers 2110
        // Credit : Inventory 1140
        // ==================================================

        var purchaseReturnProfile =
            await context.PostingProfiles
                .Include(x => x.Lines)
                .SingleOrDefaultAsync(
                    x =>
                        x.Code == "PURCHASE_RETURN_DEFAULT" &&
                        x.DocumentType == DocumentType.PurchaseReturn);

        if (purchaseReturnProfile == null)
        {
            purchaseReturnProfile = new PostingProfile(
                code: "PURCHASE_RETURN_DEFAULT",
                name: "Purchase Return - Default",
                documentType: DocumentType.PurchaseReturn,
                debitAccountId: supplierAccountId,
                creditAccountId: inventoryAccountId,
                cashFlowCategory: CashFlowCategory.None);

            context.PostingProfiles.Add(purchaseReturnProfile);

            Console.WriteLine(
                "Added Posting Profile: PURCHASE_RETURN_DEFAULT");
        }
        else
        {
            purchaseReturnProfile.Update(
                name: "Purchase Return - Default",
                debitAccountId: supplierAccountId,
                creditAccountId: inventoryAccountId,
                cashFlowCategory: CashFlowCategory.None);

            Console.WriteLine(
                "Posting Profile already exists: PURCHASE_RETURN_DEFAULT");
        }

        // ==================================================
        // Purchase Return Lines
        // ==================================================

        if (!purchaseReturnProfile.Lines.Any())
        {
            purchaseReturnProfile.AddLine(
                new PostingProfileLine(
                    sequence: 1,
                    entryType: PostingEntryType.Debit,
                    accountId: supplierAccountId,
                    amountSource: PostingAmountSource.NetTotal,
                    percentage: 100m,
                    description: "Purchase Return - Supplier"));

            purchaseReturnProfile.AddLine(
                new PostingProfileLine(
                    sequence: 2,
                    entryType: PostingEntryType.Credit,
                    accountId: inventoryAccountId,
                    amountSource: PostingAmountSource.NetTotal,
                    percentage: 100m,
                    description: "Purchase Return - Inventory"));

            Console.WriteLine(
                "Added Purchase Return Posting Profile Lines.");
        }

        await context.SaveChangesAsync();

        Console.WriteLine(
            "Posting Profiles check completed successfully.");
    }
}