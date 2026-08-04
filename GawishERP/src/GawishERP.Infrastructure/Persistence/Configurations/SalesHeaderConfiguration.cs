using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class SalesHeaderConfiguration
    : IEntityTypeConfiguration<SalesHeader>
{
    public void Configure(EntityTypeBuilder<SalesHeader> builder)
    {
        builder.ToTable("SalesHeaders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Currency)
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(x => x.ExchangeRate)
            .HasPrecision(18, 6);

        builder.Property(x => x.TotalBeforeDiscount)
            .HasPrecision(18, 2);

        builder.Property(x => x.DiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.TaxAmount)
            .HasPrecision(18, 2);

        builder.Property(x => x.NetTotal)
            .HasPrecision(18, 2);

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.SalesHeader)
            .HasForeignKey(x => x.SalesHeaderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId);
    }
}