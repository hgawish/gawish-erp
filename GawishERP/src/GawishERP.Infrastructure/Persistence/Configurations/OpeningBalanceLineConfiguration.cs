using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public class OpeningBalanceLineConfiguration
    : IEntityTypeConfiguration<OpeningBalanceLine>
{
    public void Configure(EntityTypeBuilder<OpeningBalanceLine> builder)
    {
        builder.ToTable("OpeningBalanceLines");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.UnitCost)
            .HasPrecision(18, 2);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.OpeningBalanceHeader)
            .WithMany(x => x.Lines)
            .HasForeignKey(x => x.OpeningBalanceHeaderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}