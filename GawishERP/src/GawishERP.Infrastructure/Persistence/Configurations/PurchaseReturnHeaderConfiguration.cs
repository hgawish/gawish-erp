using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public class PurchaseReturnHeaderConfiguration
    : IEntityTypeConfiguration<PurchaseReturnHeader>
{
    public void Configure(
        EntityTypeBuilder<PurchaseReturnHeader> builder)
    {
        builder.ToTable("PurchaseReturnHeaders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ReturnReason)
            .HasMaxLength(500);

        builder.Property(x => x.TotalAmount)
            .HasPrecision(18, 2);


        // Purchase

        builder.HasOne(x => x.Purchase)
            .WithMany()
            .HasForeignKey(x => x.PurchaseId)
            .OnDelete(DeleteBehavior.Restrict);


        // Supplier

        builder.HasOne(x => x.Supplier)
            .WithMany()
            .HasForeignKey(x => x.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);


        // Warehouse

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);


        // Lines

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.PurchaseReturnHeader)
            .HasForeignKey(x => x.PurchaseReturnHeaderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}