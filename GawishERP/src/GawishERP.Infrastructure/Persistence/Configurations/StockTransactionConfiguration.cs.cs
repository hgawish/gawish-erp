using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public class StockTransactionConfiguration
    : IEntityTypeConfiguration<StockTransaction>
{
    public void Configure(
        EntityTypeBuilder<StockTransaction> builder)
    {
        builder.ToTable("StockTransactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TransactionType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 3);

        builder.Property(x => x.UnitCost)
            .HasPrecision(18, 6);

        builder.Property(x => x.TransactionDate)
            .IsRequired();

        builder.Property(x => x.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new
        {
            x.WarehouseId,
            x.ProductId,
            x.TransactionDate
        });

        builder.HasIndex(x => x.TransactionType);

        builder.HasIndex(x => x.ReferenceId);
    }
}