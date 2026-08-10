using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class SalesDeliveryLineConfiguration
    : IEntityTypeConfiguration<SalesDeliveryLine>
{
    public void Configure(
        EntityTypeBuilder<SalesDeliveryLine> builder)
    {
        builder.ToTable("SalesDeliveryLines");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 3)
            .IsRequired();

        builder.HasOne(x => x.SalesOrderLine)
            .WithMany()
            .HasForeignKey(x => x.SalesOrderLineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.SalesOrderLineId);

        builder.HasIndex(x => x.ProductId);

        builder.HasIndex(x => x.WarehouseId);
    }
}