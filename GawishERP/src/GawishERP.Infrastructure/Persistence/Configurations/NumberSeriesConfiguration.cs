using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public class NumberSeriesConfiguration
    : IEntityTypeConfiguration<NumberSeries>
{
    public void Configure(EntityTypeBuilder<NumberSeries> builder)
    {
        builder.ToTable("NumberSeries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Prefix)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CurrentNumber)
            .IsRequired();

        builder.Property(x => x.Padding)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();
        builder.Property(x => x.RowVersion)
            .IsRowVersion();
        builder.HasIndex(x => x.DocumentType)
            .IsUnique();
    }
}