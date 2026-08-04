using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public class PurchaseLineConfiguration
    : IEntityTypeConfiguration<PurchaseLine>
{
    public void Configure(
        EntityTypeBuilder<PurchaseLine> builder)
    {
        builder.ToTable("PurchaseLines");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 3);

        builder.Property(x => x.UnitCost)
            .HasPrecision(18, 6);

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.LineTotal)
            .HasPrecision(18, 2);

        builder.Property(x => x.BatchNumber)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.PurchaseHeaderId);

        builder.HasIndex(x => x.ProductId);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}