using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class SalesLineConfiguration
    : IEntityTypeConfiguration<SalesLine>
{
    public void Configure(EntityTypeBuilder<SalesLine> builder)
    {
        builder.ToTable("SalesLines");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 3);

        builder.Property(x => x.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.LineTotal)
            .HasPrecision(18, 2);

        builder.Property(x => x.BatchNumber)
            .HasMaxLength(100);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId);
    }
}