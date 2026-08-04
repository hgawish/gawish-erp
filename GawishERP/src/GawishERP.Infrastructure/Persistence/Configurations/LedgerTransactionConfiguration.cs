using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class LedgerTransactionConfiguration
    : IEntityTypeConfiguration<LedgerTransaction>
{
    public void Configure(EntityTypeBuilder<LedgerTransaction> builder)
    {
        builder.ToTable("LedgerTransactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Debit)
            .HasPrecision(18, 2);

        builder.Property(x => x.Credit)
            .HasPrecision(18, 2);

        builder.Property(x => x.RunningBalance)
            .HasPrecision(18, 2);

        builder.HasIndex(x => new
        {
            x.AccountId,
            x.PostingDate
        });

        builder.HasIndex(x => x.DocumentNumber);

        builder.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.JournalEntryHeader)
            .WithMany()
            .HasForeignKey(x => x.JournalEntryHeaderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.JournalEntryLine)
            .WithMany()
            .HasForeignKey(x => x.JournalEntryLineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FiscalYear)
            .WithMany()
            .HasForeignKey(x => x.FiscalYearId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}