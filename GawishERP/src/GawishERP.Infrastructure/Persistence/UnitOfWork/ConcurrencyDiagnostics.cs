using Microsoft.EntityFrameworkCore;
using System.Text;

namespace GawishERP.Infrastructure.Persistence.UnitOfWork;

internal static class ConcurrencyDiagnostics
{
    public static async Task<string> BuildAsync(
        DbContext context,
        CancellationToken cancellationToken = default)
    {
        var sb = new StringBuilder();

        foreach (var entry in context.ChangeTracker.Entries()
                     .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
        {
            sb.AppendLine($"Entity={entry.Metadata.ClrType.FullName}, State={entry.State}");

            foreach (var key in entry.Properties.Where(p => p.Metadata.IsPrimaryKey()))
            {
                sb.AppendLine($"  Key={key.Metadata.Name}, Value={key.CurrentValue}");
            }

            foreach (var property in entry.Properties)
            {
                if (property.Metadata.IsConcurrencyToken)
                {
                    sb.AppendLine($"  Concurrency={property.Metadata.Name}, Original={property.OriginalValue}, Current={property.CurrentValue}");
                }
            }

            if (entry.State is EntityState.Modified or EntityState.Deleted)
            {
                var databaseValues = await entry.GetDatabaseValuesAsync(cancellationToken);
                sb.AppendLine($"  DatabaseRowExists={databaseValues is not null}");

                if (databaseValues is not null)
                {
                    foreach (var property in entry.Properties)
                    {
                        if (property.Metadata.IsConcurrencyToken)
                        {
                            sb.AppendLine($"  DatabaseConcurrency={property.Metadata.Name}, Value={databaseValues[property.Metadata.Name]}");
                        }
                    }
                }
            }
        }

        return sb.ToString();
    }
}
