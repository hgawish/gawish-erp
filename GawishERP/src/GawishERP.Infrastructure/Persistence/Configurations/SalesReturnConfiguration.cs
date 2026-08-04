using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class SalesReturnHeaderConfiguration
    : IEntityTypeConfiguration<SalesReturnHeader>
{
    public void Configure(EntityTypeBuilder<SalesReturnHeader> builder)
    {
        builder.ToTable("SalesReturnHeaders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ReturnReason)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.NoAction);

        // ===== الحل هنا =====
        builder.HasOne(x => x.Sales)
            .WithMany()
            .HasForeignKey(x => x.SalesId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.SalesReturnHeader)
            .HasForeignKey(x => x.SalesReturnHeaderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class SalesReturnLineConfiguration
    : IEntityTypeConfiguration<SalesReturnLine>
{
    public void Configure(EntityTypeBuilder<SalesReturnLine> builder)
    {
        builder.ToTable("SalesReturnLines");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.LineTotal)
            .HasPrecision(18, 2);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}