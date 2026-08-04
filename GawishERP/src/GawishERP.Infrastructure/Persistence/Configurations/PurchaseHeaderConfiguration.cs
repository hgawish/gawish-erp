using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public class PurchaseHeaderConfiguration
    : IEntityTypeConfiguration<PurchaseHeader>
{
    public void Configure(
        EntityTypeBuilder<PurchaseHeader> builder)
    {
        builder.ToTable("PurchaseHeaders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.DocumentNumber)
            .IsUnique();

        builder.Property(x => x.InvoiceNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.ExchangeRate)
            .HasPrecision(18, 8);

        builder.Property(x => x.TotalBeforeDiscount)
            .HasPrecision(18, 2);

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.NetTotal)
            .HasPrecision(18, 2);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .HasDefaultValue(DocumentStatus.Draft)
            .IsRequired();

        builder.HasIndex(x => x.SupplierId);

        builder.HasIndex(x => x.WarehouseId);

        builder.HasIndex(x => x.DocumentDate);

        builder.HasIndex(x => x.InvoiceDate);

        builder.HasOne(x => x.Supplier)
            .WithMany()
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.PurchaseHeader)
            .HasForeignKey(x => x.PurchaseHeaderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}