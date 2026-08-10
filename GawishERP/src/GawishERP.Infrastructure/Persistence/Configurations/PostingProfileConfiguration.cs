using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class PostingProfileConfiguration
    : IEntityTypeConfiguration<PostingProfile>
{
    public void Configure(
        EntityTypeBuilder<PostingProfile> builder)
    {
        builder.ToTable("PostingProfiles");

        //=========================================================
        // Primary Key
        //=========================================================

        builder.HasKey(x => x.Id);

        //=========================================================
        // Code
        //=========================================================

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Code)
            .IsUnique();

        //=========================================================
        // Name
        //=========================================================

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        //=========================================================
        // Document Type
        //=========================================================

        builder.Property(x => x.DocumentType)
            .HasConversion<int>()
            .IsRequired();

        //=========================================================
        // Cash Flow Category
        //=========================================================

        builder.Property(x => x.CashFlowCategory)
            .HasConversion<int>()
            .IsRequired();

        //=========================================================
        // Active
        //=========================================================

        builder.Property(x => x.IsActive)
            .IsRequired();

        //=========================================================
        // Legacy / Header Debit Account
        //=========================================================

        builder.HasOne(x => x.DebitAccount)
            .WithMany()
            .HasForeignKey(x => x.DebitAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        //=========================================================
        // Legacy / Header Credit Account
        //=========================================================

        builder.HasOne(x => x.CreditAccount)
            .WithMany()
            .HasForeignKey(x => x.CreditAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        //=========================================================
        // Posting Profile Lines
        //=========================================================

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.PostingProfile)
            .HasForeignKey(x => x.PostingProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}