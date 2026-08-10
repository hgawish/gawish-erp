using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class SalesQuotationLineConfiguration
    : IEntityTypeConfiguration<SalesQuotationLine>
{
    public void Configure(
        EntityTypeBuilder<SalesQuotationLine> builder)
    {
        builder.ToTable("SalesQuotationLines");

        //====================================================
        // Primary Key
        //====================================================

        builder.HasKey(x => x.Id);

        //====================================================
        // Quantity & Prices
        //====================================================

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.DiscountPercent)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.TaxPercent)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.TaxAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.LineSubTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(x => x.LineTotal)
            .HasPrecision(18, 2)
            .IsRequired();

        //====================================================
        // Audit
        //====================================================

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy);

        builder.Property(x => x.UpdatedAt);

        builder.Property(x => x.UpdatedBy);

        //====================================================
        // Sales Quotation
        //====================================================

        builder.HasOne(x => x.SalesQuotation)
            .WithMany(x => x.Lines)
            .HasForeignKey(x => x.SalesQuotationId)
            .OnDelete(DeleteBehavior.Cascade);

        //====================================================
        // Product
        //====================================================

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        //====================================================
        // Indexes
        //====================================================

        builder.HasIndex(x => x.SalesQuotationId);

        builder.HasIndex(x => x.ProductId);
    }
}