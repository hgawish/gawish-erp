using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class SalesOrderConfiguration
    : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(
        EntityTypeBuilder<SalesOrder> builder)
    {
        builder.ToTable("SalesOrders");

        builder.HasKey(x => x.Id);

        //====================================================
        // Document
        //====================================================

        builder.Property(x => x.DocumentNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.Property(x => x.Status)
            .IsRequired();

        //====================================================
        // Totals
        //====================================================

        builder.Property(x => x.TotalBeforeDiscount)
            .HasPrecision(18, 2);

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalAfterDiscount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.NetAmount)
            .HasPrecision(18, 2);

        //====================================================
        // Customer
        //====================================================

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        //====================================================
        // Sales Quotation
        //====================================================

        builder.HasOne(x => x.SalesQuotation)
            .WithMany()
            .HasForeignKey(x => x.SalesQuotationId)
            .OnDelete(DeleteBehavior.Restrict);

        //====================================================
        // Lines
        //====================================================

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.SalesOrder)
            .HasForeignKey(x => x.SalesOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        //====================================================
        // Indexes
        //====================================================

        builder.HasIndex(x => x.DocumentNumber)
            .IsUnique();

        builder.HasIndex(x => x.DocumentDate);

        builder.HasIndex(x => x.CustomerId);

        builder.HasIndex(x => x.SalesQuotationId);
    }
}