using Microsoft.EntityFrameworkCore;
using Zefir.Core.Analytics.Metrics;

namespace Zefir.DAL.Analytics;

public sealed class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Metric> Metrics { get; set; } = null!;
}
