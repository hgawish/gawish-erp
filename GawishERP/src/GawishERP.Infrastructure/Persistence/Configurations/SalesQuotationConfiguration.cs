using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class SalesQuotationConfiguration
    : IEntityTypeConfiguration<SalesQuotation>
{
    public void Configure(
        EntityTypeBuilder<SalesQuotation> builder)
    {
        builder.ToTable("SalesQuotations");

        //====================================================
        // Primary Key
        //====================================================

        builder.HasKey(x => x.Id);

        //====================================================
        // Quotation
        //====================================================

        builder.Property(x => x.QuotationNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => x.QuotationNumber)
            .IsUnique();

        builder.Property(x => x.QuotationDate)
            .IsRequired();

        builder.Property(x => x.Status)
            .IsRequired();

        //====================================================
        // Amounts
        //====================================================

        builder.Property(x => x.SubTotal)
            .HasPrecision(18, 2);

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);

        //====================================================
        // Remarks
        //====================================================

        builder.Property(x => x.Remarks)
            .HasMaxLength(500);

        //====================================================
        // Audit
        //====================================================

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy);

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.UpdatedBy);

        //====================================================
        // Customer
        //====================================================

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        //====================================================
        // Warehouse
        //====================================================

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        //====================================================
        // Lines
        //====================================================

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.SalesQuotation)
            .HasForeignKey(x => x.SalesQuotationId)
            .OnDelete(DeleteBehavior.Cascade);

        //====================================================
        // Indexes
        //====================================================

        builder.HasIndex(x => x.CustomerId);

        builder.HasIndex(x => x.WarehouseId);

        builder.HasIndex(x => x.QuotationDate);

        builder.HasIndex(x => x.Status);
    }
}