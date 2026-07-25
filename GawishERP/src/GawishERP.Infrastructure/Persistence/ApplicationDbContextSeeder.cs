using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace GawishERP.Infrastructure.Persistence;

public static class ApplicationDbContextSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        // ===========================
        // Roles
        // ===========================

        if (!context.Roles.Any())
        {
            var superAdmin = new Role("SuperAdmin", "System Administrator");
            var admin = new Role("Admin", "Administrator");
            var sales = new Role("Sales", "Sales Department");
            var warehouse = new Role("Warehouse", "Warehouse Department");
            var accountant = new Role("Accountant", "Accounting Department");

            context.Roles.AddRange(
                superAdmin,
                admin,
                sales,
                warehouse,
                accountant);

            await context.SaveChangesAsync();
        }

        // ===========================
        // Permissions
        // ===========================

        if (!context.Permissions.Any())
        {
            var permissions = new List<Permission>
            {
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

                new("Sales.View", "View Sales"),
                new("Sales.Create", "Create Sales"),
                new("Sales.Update", "Update Sales"),

                new("Purchases.View", "View Purchases"),
                new("Purchases.Create", "Create Purchases"),

                new("Inventory.View", "View Inventory"),
                new("Inventory.Update", "Update Inventory")
            };

            context.Permissions.AddRange(permissions);

            await context.SaveChangesAsync();
        }// ===========================
         // SuperAdmin Permissions
         // ===========================


        var superAdminRole = await context.Roles
            .FirstOrDefaultAsync(x => x.Name == "SuperAdmin");

        if (superAdminRole is not null)
        {
            var allPermissions = await context.Permissions.ToListAsync();

            foreach (var permission in allPermissions)
            {
                bool exists = await context.RolePermissions.AnyAsync(x =>
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
            // ===========================
            // Super Admin User
            // ===========================

            var adminUser = await context.Users
                .FirstOrDefaultAsync(x => x.Email == "admin@gawisherp.com");

            if (adminUser is null && superAdminRole is not null)
            {
                var user = new User(
                    "System",
                    "Administrator",
                    "admin@gawisherp.com",
                    BCrypt.Net.BCrypt.HashPassword("Admin@123"));

                context.Users.Add(user);

                await context.SaveChangesAsync();

                context.UserRoles.Add(
                    new UserRole(
                        user.Id,
                        superAdminRole.Id));

                await context.SaveChangesAsync();
            }
            await context.SaveChangesAsync();
        }
    }
}