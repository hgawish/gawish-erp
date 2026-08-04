using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Code)
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .HasMaxLength(200);

        builder.Property(x => x.ArabicName)
            .HasMaxLength(200);

        builder.Property(x => x.Manager)
            .HasMaxLength(200);

        builder.Property(x => x.Phone)
            .HasMaxLength(50);

        builder.Property(x => x.Address)
            .HasMaxLength(1000);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);
    }
}