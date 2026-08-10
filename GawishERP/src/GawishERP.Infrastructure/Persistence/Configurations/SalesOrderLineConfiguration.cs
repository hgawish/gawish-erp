using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class SalesOrderLineConfiguration
    : IEntityTypeConfiguration<SalesOrderLine>
{
    public void Configure(
        EntityTypeBuilder<SalesOrderLine> builder)
    {
        builder.ToTable("SalesOrderLines");

        builder.HasKey(x => x.Id);

        //====================================================
        // Decimal Properties
        //====================================================

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.DiscountPercent)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.TaxPercent)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.TaxAmount)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.LineTotalBeforeDiscount)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.LineTotalAfterDiscount)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.NetAmount)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.DeliveredQuantity)
            .HasPrecision(18, 4)
            .IsRequired();

        builder.Property(x => x.InvoicedQuantity)
            .HasPrecision(18, 4)
            .IsRequired();

        //====================================================
        // Sales Order
        //====================================================

        builder.HasOne(x => x.SalesOrder)
            .WithMany(x => x.Lines)
            .HasForeignKey(x => x.SalesOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        //====================================================
        // Product
        //====================================================

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        //====================================================
        // Warehouse
        //====================================================

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        //====================================================
        // Indexes
        //====================================================

        builder.HasIndex(x => x.ProductId);

        builder.HasIndex(x => x.WarehouseId);

        builder.HasIndex(x => x.SalesOrderId);
    }
}