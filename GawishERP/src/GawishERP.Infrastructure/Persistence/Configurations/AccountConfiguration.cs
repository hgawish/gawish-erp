using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public class AccountConfiguration
    : IEntityTypeConfiguration<Account>
{
    public void Configure(
        EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Accounts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.AccountType)
            .IsRequired();

        builder.Property(x => x.Nature)
            .IsRequired();

        builder.Property(x => x.IsPostingAccount)
            .IsRequired();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.HasOne(x => x.ParentAccount)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentAccountId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}