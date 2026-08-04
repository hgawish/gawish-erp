using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public class InventoryBalanceConfiguration
    : IEntityTypeConfiguration<InventoryBalance>
{
    public void Configure(EntityTypeBuilder<InventoryBalance> builder)
    {
        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.AverageCost)
            .HasPrecision(18, 2);

        builder.HasIndex(x => new
        {
            x.ProductId,
            x.WarehouseId
        })
        .IsUnique();

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}