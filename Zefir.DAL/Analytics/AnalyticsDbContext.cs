using Microsoft.EntityFrameworkCore;

namespace Zefir.DAL.Analytics;

public sealed class AnalyticsDbContext : DbContext
{
    public AnalyticsDbContext(DbContextOptions<AnalyticsDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}
