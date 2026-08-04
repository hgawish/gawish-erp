using Microsoft.EntityFrameworkCore;

namespace GawishERP.Infrastructure.Persistence.Seed;

public static class ApplicationDbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SeedNumberSeriesAsync(context);
    }

    private static async Task SeedNumberSeriesAsync(ApplicationDbContext context)
    {
        if (await context.NumberSeries.AnyAsync())
            return;

        var series = NumberSeriesSeeder.GetDefaultSeries();

        await context.NumberSeries.AddRangeAsync(series);

        await context.SaveChangesAsync();
    }
}