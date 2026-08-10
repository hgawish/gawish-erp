using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class PostingProfileLineConfiguration
    : IEntityTypeConfiguration<PostingProfileLine>
{
    public void Configure(
        EntityTypeBuilder<PostingProfileLine> builder)
    {
        builder.ToTable("PostingProfileLines");

        //=========================================================
        // Primary Key
        //=========================================================

        builder.HasKey(x => x.Id);

        //=========================================================
        // Posting Profile Id
        //=========================================================

        builder.Property(x => x.PostingProfileId)
            .IsRequired();

        //=========================================================
        // Sequence
        //=========================================================

        builder.Property(x => x.Sequence)
            .IsRequired();

        builder.HasIndex(x => new
        {
            x.PostingProfileId,
            x.Sequence
        })
        .IsUnique();

        //=========================================================
        // Entry Type
        //=========================================================

        builder.Property(x => x.EntryType)
            .HasConversion<int>()
            .IsRequired();

        //=========================================================
        // Account
        //=========================================================

        builder.Property(x => x.AccountId)
            .IsRequired();

        builder.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        //=========================================================
        // Amount Source
        //=========================================================

        builder.Property(x => x.AmountSource)
            .HasConversion<int>()
            .IsRequired();

        //=========================================================
        // Percentage
        //=========================================================

        builder.Property(x => x.Percentage)
            .HasPrecision(18, 4)
            .IsRequired();

        //=========================================================
        // Description
        //=========================================================

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        //=========================================================
        // Posting Profile
        //=========================================================

        builder.HasOne(x => x.PostingProfile)
            .WithMany(x => x.Lines)
            .HasForeignKey(x => x.PostingProfileId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}