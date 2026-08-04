using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Code)
            .IsUnique();

        builder.Property(x => x.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.ArabicName)
            .HasMaxLength(200);

        builder.Property(x => x.ContactPerson)
            .HasMaxLength(200);

        builder.Property(x => x.Phone)
            .HasMaxLength(50);

        builder.Property(x => x.Mobile)
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .HasMaxLength(200);

        builder.Property(x => x.TaxNumber)
            .HasMaxLength(100);

        builder.Property(x => x.CommercialRegistration)
            .HasMaxLength(100);

        builder.Property(x => x.Address)
            .HasMaxLength(1000);

        builder.Property(x => x.City)
            .HasMaxLength(100);

        builder.Property(x => x.Country)
            .HasMaxLength(100);

        builder.Property(x => x.Notes)
            .HasMaxLength(2000);

        // ============================================
        // Accounting
        // ============================================

        builder.HasOne(x => x.Account)
            .WithMany()
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}