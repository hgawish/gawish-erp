using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<UserRole> UserRoles => Set<UserRole>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    // Products
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ============================
        // UserRole
        // ============================

        modelBuilder.Entity<UserRole>()
            .HasKey(x => new { x.UserId, x.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(x => x.User)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(x => x.Role)
            .WithMany(x => x.UserRoles)
            .HasForeignKey(x => x.RoleId);

        // ============================
        // RolePermission
        // ============================

        modelBuilder.Entity<RolePermission>()
            .HasKey(x => new { x.RoleId, x.PermissionId });

        modelBuilder.Entity<RolePermission>()
            .HasOne(x => x.Role)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.RoleId);

        modelBuilder.Entity<RolePermission>()
            .HasOne(x => x.Permission)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.PermissionId);

        // ============================
        // User
        // ============================

        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        // ============================
        // Role
        // ============================

        modelBuilder.Entity<Role>()
            .HasIndex(x => x.Name)
            .IsUnique();

        // ============================
        // Permission
        // ============================

        modelBuilder.Entity<Permission>()
            .HasIndex(x => x.Name)
            .IsUnique();

        // ============================
        // Product
        // ============================

        modelBuilder.Entity<Product>()
            .HasIndex(x => x.Code)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .Property(x => x.Code)
            .HasMaxLength(50);

        modelBuilder.Entity<Product>()
            .Property(x => x.Name)
            .HasMaxLength(200);

        modelBuilder.Entity<Product>()
            .Property(x => x.ArabicName)
            .HasMaxLength(200);

        modelBuilder.Entity<Product>()
            .Property(x => x.Description)
            .HasMaxLength(1000);

        modelBuilder.Entity<Product>()
            .Property(x => x.CostPrice)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Product>()
            .Property(x => x.SalePrice)
            .HasPrecision(18, 2);
    }
}