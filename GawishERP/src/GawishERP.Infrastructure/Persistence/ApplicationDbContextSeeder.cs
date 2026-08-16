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
            new("SuperAdmin", "System Administrator"), new("Admin", "Administrator"), new("Sales", "Sales Department"), new("Warehouse", "Warehouse Department"), new("Accountant", "Accounting Department")
        };
        foreach (var role in roles) if (!await context.Roles.AnyAsync(x => x.Name == role.Name)) context.Roles.Add(role);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPermissionsAsync(ApplicationDbContext context)
    {
        var permissions = new List<Permission>
        {
            new("Warehouses.View", "View Warehouses"), new("Warehouses.Create", "Create Warehouses"), new("Warehouses.Edit", "Edit Warehouses"), new("Warehouses.Delete", "Delete Warehouses"), new("Users.View", "View Users"), new("Users.Create", "Create Users"), new("Users.Update", "Update Users"), new("Users.Delete", "Delete Users"), new("Roles.View", "View Roles"), new("Roles.Manage", "Manage Roles"), new("Products.View", "View Products"), new("Products.Create", "Create Products"), new("Products.Update", "Update Products"), new("Products.Delete", "Delete Products"), new("Customers.View", "View Customers"), new("Customers.Create", "Create Customers"), new("Customers.Update", "Update Customers"), new("Customers.Delete", "Delete Customers"), new("Suppliers.View", "View Suppliers"), new("Suppliers.Create", "Create Suppliers"), new("Suppliers.Edit", "Edit Suppliers"), new("Suppliers.Delete", "Delete Suppliers"), new("Sales.View", "View Sales"), new("Sales.Create", "Create Sales"), new("Sales.Update", "Update Sales"), new("Purchases.View", "View Purchases"), new("Purchases.Create", "Create Purchases"), new("Inventory.View", "View Inventory"), new("Inventory.Update", "Update Inventory")
        };
        foreach (var permission in permissions) if (!await context.Permissions.AnyAsync(x => x.Name == permission.Name)) context.Permissions.Add(permission);
        await context.SaveChangesAsync();
    }

    private static async Task SeedSuperAdminRolePermissionsAsync(ApplicationDbContext context)
    {
        var role = await context.Roles.SingleAsync(x => x.Name == "SuperAdmin");
        var permissions = await context.Permissions.ToListAsync();
        foreach (var permission in permissions) if (!await context.RolePermissions.AnyAsync(x => x.RoleId == role.Id && x.PermissionId == permission.Id)) context.RolePermissions.Add(new RolePermission(role.Id, permission.Id));
        await context.SaveChangesAsync();
    }

    private static async Task SeedAdminUserAsync(ApplicationDbContext context)
    {
        var adminUser = await context.Users.SingleOrDefaultAsync(x => x.Email == "admin@gawisherp.com");
        if (adminUser == null)
        {
            adminUser = new User("System", "Administrator", "admin@gawisherp.com", BCrypt.Net.BCrypt.HashPassword("Admin@123"));
            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }
        var role = await context.Roles.SingleAsync(x => x.Name == "SuperAdmin");
        if (!await context.UserRoles.AnyAsync(x => x.UserId == adminUser.Id && x.RoleId == role.Id))
        {
            context.UserRoles.Add(new UserRole(adminUser.Id, role.Id));
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedNumberSeriesAsync(ApplicationDbContext context)
    {
        var defaultSeries = new List<NumberSeries>
        {
            new(DocumentType.OpeningBalance, "OB"), new(DocumentType.Purchase, "PO"), new(DocumentType.PurchaseReturn, "PR"), new(DocumentType.Sales, "SO"), new(DocumentType.SalesReturn, "SR"), new(DocumentType.Transfer, "TR"), new(DocumentType.Adjustment, "ADJ"), new(DocumentType.Production, "MO"), new(DocumentType.StockCount, "SC")
        };
        foreach (var series in defaultSeries) if (!await context.NumberSeries.AnyAsync(x => x.DocumentType == series.DocumentType)) context.NumberSeries.Add(series);
        await context.SaveChangesAsync();
    }

    private static async Task SeedPostingProfilesAsync(ApplicationDbContext context)
    {
        async Task<Guid> ResolveAccountIdAsync(string code, string name)
        {
            var account = await context.Accounts.AsNoTracking().SingleOrDefaultAsync(x => x.Code == code);
            if (account is null) throw new InvalidOperationException($"Required system account '{code} - {name}' was not found.");
            if (!account.IsPostingAccount) throw new InvalidOperationException($"Required system account '{code} - {name}' does not allow posting.");
            return account.Id;
        }

        var inventory = await ResolveAccountIdAsync("1140", "Inventory");
        var customer = await ResolveAccountIdAsync("1130", "Customers");
        var supplier = await ResolveAccountIdAsync("2110", "Suppliers");
        var sales = await ResolveAccountIdAsync("4100", "Sales Revenue");
        var salesReturns = await ResolveAccountIdAsync("4200", "Sales Returns");
        var cogs = await ResolveAccountIdAsync("5100", "Cost Of Sales");

        async Task ReplaceLinesAsync(PostingProfile profile, params PostingProfileLine[] lines)
        {
            // Do NOT call SaveChanges before this delete. BaseEntity generates
            // profile.Id client-side, so new child rows can reference it safely.
            // Existing profile header changes and new lines are persisted together.
            await context.Set<PostingProfileLine>()
                .Where(x => x.PostingProfileId == profile.Id)
                .ExecuteDeleteAsync();

            // Lines were intentionally not loaded, so the in-memory collection
            // does not contain rows deleted by ExecuteDeleteAsync.
            profile.ClearLines();
            foreach (var line in lines) profile.AddLine(line);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine("========== POSTING PROFILE CONCURRENCY ==========");

                foreach (var entry in ex.Entries)
                {
                    Console.WriteLine($"Entity: {entry.Metadata.ClrType.Name}");
                    Console.WriteLine($"State : {entry.State}");

                    var key = entry.Metadata.FindPrimaryKey();

                    if (key != null)
                    {
                        foreach (var property in key.Properties)
                        {
                            Console.WriteLine(
                                $"Key {property.Name}: {entry.Property(property.Name).CurrentValue}");
                        }
                    }

                    Console.WriteLine("------------------------------------------------");
                }

                throw;
            }
        }

        async Task<PostingProfile> GetOrCreateAsync(string code, string name, DocumentType type, Guid debit, Guid credit, CashFlowCategory cashFlow)
        {
            var profile = await context.PostingProfiles.SingleOrDefaultAsync(x => x.Code == code && x.DocumentType == type);
            if (profile is null)
            {
                profile = new PostingProfile(code, name, type, debit, credit, cashFlow);
                context.PostingProfiles.Add(profile);
            }
            else profile.Update(name, debit, credit, cashFlow);
            return profile;
        }

        var purchase = await GetOrCreateAsync("PURCHASE_DEFAULT", "Purchase - Default", DocumentType.Purchase, inventory, supplier, CashFlowCategory.None);
        await ReplaceLinesAsync(purchase,
            new PostingProfileLine(1, PostingEntryType.Debit, inventory, PostingAmountSource.NetTotal, 100m, "Purchase - Inventory"),
            new PostingProfileLine(2, PostingEntryType.Credit, supplier, PostingAmountSource.NetTotal, 100m, "Purchase - Supplier"));

        var purchaseReturn = await GetOrCreateAsync("PURCHASE_RETURN_DEFAULT", "Purchase Return - Default", DocumentType.PurchaseReturn, supplier, inventory, CashFlowCategory.None);
        await ReplaceLinesAsync(purchaseReturn,
            new PostingProfileLine(1, PostingEntryType.Debit, supplier, PostingAmountSource.NetTotal, 100m, "Purchase Return - Supplier"),
            new PostingProfileLine(2, PostingEntryType.Credit, inventory, PostingAmountSource.NetTotal, 100m, "Purchase Return - Inventory"));

        var sale = await GetOrCreateAsync("SALES_DEFAULT", "Sales - Default", DocumentType.Sales, customer, sales, CashFlowCategory.Operating);
        await ReplaceLinesAsync(sale,
            new PostingProfileLine(1, PostingEntryType.Debit, customer, PostingAmountSource.NetTotal, 100m, "Sales - Customer"),
            new PostingProfileLine(2, PostingEntryType.Credit, sales, PostingAmountSource.NetTotal, 100m, "Sales - Revenue"),
            new PostingProfileLine(3, PostingEntryType.Debit, cogs, PostingAmountSource.CostOfGoodsSold, 100m, "Sales - COGS"),
            new PostingProfileLine(4, PostingEntryType.Credit, inventory, PostingAmountSource.CostOfGoodsSold, 100m, "Sales - Inventory"));

        var salesReturn = await GetOrCreateAsync("SALES_RETURN_DEFAULT", "Sales Return - Default", DocumentType.SalesReturn, salesReturns, customer, CashFlowCategory.Operating);
        await ReplaceLinesAsync(salesReturn,
            new PostingProfileLine(1, PostingEntryType.Debit, salesReturns, PostingAmountSource.NetTotal, 100m, "Sales Return - Revenue"),
            new PostingProfileLine(2, PostingEntryType.Credit, customer, PostingAmountSource.NetTotal, 100m, "Sales Return - Customer"),
            new PostingProfileLine(3, PostingEntryType.Debit, inventory, PostingAmountSource.CostOfGoodsSold, 100m, "Sales Return - Inventory"),
            new PostingProfileLine(4, PostingEntryType.Credit, cogs, PostingAmountSource.CostOfGoodsSold, 100m, "Sales Return - COGS"));
    }
}
