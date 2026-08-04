using GawishERP.Domain.Common;
using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public class OpeningBalanceHeaderConfiguration
    : IEntityTypeConfiguration<OpeningBalanceHeader>
{
    public void Configure(EntityTypeBuilder<OpeningBalanceHeader> builder)
    {
        builder.ToTable("OpeningBalanceHeaders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.DocumentNumber)
            .IsUnique();

        builder.Property(x => x.DocumentDate)
            .IsRequired();

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        // بديل IsPosted
        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.HasOne(x => x.Warehouse)
            .WithMany()
            .HasForeignKey(x => x.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.OpeningBalanceHeader)
            .HasForeignKey(x => x.OpeningBalanceHeaderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}