using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class FinancialStatementNodeConfiguration
    : IEntityTypeConfiguration<FinancialStatementNode>
{
    public void Configure(
        EntityTypeBuilder<FinancialStatementNode> builder)
    {
        builder.ToTable("AccountReportCategories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.StatementType)
            .IsRequired();

        builder.Property(x => x.NormalBalance)
            .IsRequired();

        builder.Property(x => x.SortOrder)
            .IsRequired();

        builder.Property(x => x.IsSystem)
            .IsRequired();

        builder.Property(x => x.IsEditable)
            .IsRequired();

        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Formula)
    .HasMaxLength(500);

        builder.Property(x => x.IsHeader)
            .IsRequired();

        builder.Property(x => x.IsTotal)
            .IsRequired();

        builder.Property(x => x.AllowPosting)
            .IsRequired();

        builder.Property(x => x.IsVisible)
            .IsRequired();

        builder.Property(x => x.Level)
            .IsRequired();

        builder.Property(x => x.SortOrder)
            .IsRequired();
    }
}