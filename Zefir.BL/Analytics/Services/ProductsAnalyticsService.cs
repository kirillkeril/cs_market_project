using Zefir.DAL;
using Zefir.DAL.Analytics;

namespace Zefir.BL.Analytics.Services;

public class ProductsAnalyticsService
{
    private readonly AnalyticsDbContext _analyticsDbContext;
    private readonly AppDbContext _appDbContext;

    public ProductsAnalyticsService(AnalyticsDbContext dbContext, AppDbContext appDbContext)
    {
        _analyticsDbContext = dbContext;
        _appDbContext = appDbContext;
    }
}
