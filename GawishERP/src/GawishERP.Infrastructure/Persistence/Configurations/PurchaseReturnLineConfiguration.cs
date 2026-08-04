using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public class PurchaseReturnLineConfiguration
    : IEntityTypeConfiguration<PurchaseReturnLine>
{
    public void Configure(
        EntityTypeBuilder<PurchaseReturnLine> builder)
    {
        builder.ToTable("PurchaseReturnLines");


        builder.HasKey(x => x.Id);


        builder.Property(x => x.Quantity)
            .HasPrecision(18, 3);


        builder.Property(x => x.UnitCost)
            .HasPrecision(18, 2);


        builder.Property(x => x.LineTotal)
            .HasPrecision(18, 2);



        // Purchase Line Reference

        builder.HasOne(x => x.PurchaseLine)
            .WithMany()
            .HasForeignKey(x => x.PurchaseLineId)
            .OnDelete(DeleteBehavior.Restrict);



        // Product

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}