using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class SalesDeliveryConfiguration
    : IEntityTypeConfiguration<SalesDelivery>
{
    public void Configure(
        EntityTypeBuilder<SalesDelivery> builder)
    {
        builder.ToTable("SalesDeliveries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.HasIndex(x => x.DocumentNumber)
            .IsUnique();

        builder.Property(x => x.DocumentDate)
            .IsRequired();

        builder.Property(x => x.TotalQuantity)
            .HasPrecision(18, 3);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SalesOrder)
            .WithMany()
            .HasForeignKey(x => x.SalesOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.SalesDelivery)
            .HasForeignKey(x => x.SalesDeliveryId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}