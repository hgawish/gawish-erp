using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public sealed class AccountBalanceConfiguration
    : IEntityTypeConfiguration<AccountBalance>
{
    public void Configure(EntityTypeBuilder<AccountBalance> builder)
    {
        builder.ToTable("AccountBalances");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OpeningDebit)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.OpeningCredit)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.CurrentDebit)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.CurrentCredit)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.ClosingBalance)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.HasIndex(x => new
        {
            x.AccountId,
            x.FiscalYearId,
            x.CompanyId,
            x.BranchId
        })
        .IsUnique();

        builder.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.FiscalYear)
            .WithMany()
            .HasForeignKey(x => x.FiscalYearId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}