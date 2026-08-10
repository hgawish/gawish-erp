using GawishERP.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Seed;

public static class ApplicationDbSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext context)
    {
        await SeedNumberSeriesAsync(context);
    }

    //=========================================================
    // Number Series
    //=========================================================

    private static async Task SeedNumberSeriesAsync(
        ApplicationDbContext context)
    {
        var defaultSeries =
            NumberSeriesSeeder.GetDefaultSeries();

        foreach (var series in defaultSeries)
        {
            var exists =
                await context.NumberSeries
                    .AnyAsync(
                        x => x.DocumentType == series.DocumentType);

            if (exists)
                continue;

            await context.NumberSeries.AddAsync(series);
        }

        await context.SaveChangesAsync();
    }
}