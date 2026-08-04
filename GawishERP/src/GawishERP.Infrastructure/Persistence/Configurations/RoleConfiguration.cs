using GawishERP.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GawishERP.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasIndex(x => x.Name)
               .IsUnique();

        builder.Property(x => x.Name)
               .HasMaxLength(100);

        builder.Property(x => x.Description)
               .HasMaxLength(500);
    }
}