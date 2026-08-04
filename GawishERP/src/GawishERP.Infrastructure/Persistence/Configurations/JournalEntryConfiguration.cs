using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class JournalEntryConfiguration
    : IEntityTypeConfiguration<JournalEntryHeader>
{
    public void Configure(EntityTypeBuilder<JournalEntryHeader> builder)
    {
        builder.ToTable("JournalEntryHeaders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ReferenceNumber)
            .HasMaxLength(100);

        builder.Property(x => x.TotalDebit)
            .HasPrecision(18, 2);

        builder.Property(x => x.TotalCredit)
            .HasPrecision(18, 2);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        //==========================
        // Lines
        //==========================

        builder.HasMany(x => x.Lines)
            .WithOne(x => x.JournalEntryHeader)
            .HasForeignKey(x => x.JournalEntryHeaderId)
            .OnDelete(DeleteBehavior.Cascade);

        //==========================
        // Self Reference
        // Original Journal Entry
        //==========================

        builder.HasOne(x => x.OriginalJournalEntry)
            .WithOne(x => x.ReversedByJournalEntry)
            .HasForeignKey<JournalEntryHeader>(
                x => x.OriginalJournalEntryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public sealed class JournalEntryLineConfiguration
    : IEntityTypeConfiguration<JournalEntryLine>
{
    public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
    {
        builder.ToTable("JournalEntryLines");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Debit)
            .HasPrecision(18, 2);

        builder.Property(x => x.Credit)
            .HasPrecision(18, 2);

        builder.Property(x => x.Description)
            .HasMaxLength(250);

        builder.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}