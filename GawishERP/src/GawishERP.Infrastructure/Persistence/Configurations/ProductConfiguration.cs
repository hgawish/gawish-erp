using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Code)
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .HasMaxLength(200);

        builder.Property(x => x.ArabicName)
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.CostPrice)
            .HasPrecision(18, 2);

        builder.Property(x => x.SalePrice)
            .HasPrecision(18, 2);
    }
}