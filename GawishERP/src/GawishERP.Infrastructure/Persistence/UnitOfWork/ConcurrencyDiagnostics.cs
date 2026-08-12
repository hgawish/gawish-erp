using Microsoft.EntityFrameworkCore;
using System.Text;

namespace GawishERP.Infrastructure.Persistence.UnitOfWork;

internal static class ConcurrencyDiagnostics
{
    public static string Build(DbContext context)
    {
        var sb = new StringBuilder();

        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            sb.AppendLine($"Entity={entry.Metadata.ClrType.FullName}, State={entry.State}");

            foreach (var property in entry.Properties)
            {
                if (property.Metadata.IsConcurrencyToken)
                {
                    sb.AppendLine($"  Concurrency={property.Metadata.Name}, Original={property.OriginalValue}, Current={property.CurrentValue}");
                }
            }
        }

        return sb.ToString();
    }
}
