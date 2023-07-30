namespace Zefir.DAL.Services;

public class OrderService
{
    private AppDbContext _appDbContext;

    public OrderService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
}
