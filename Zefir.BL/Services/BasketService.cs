using Microsoft.EntityFrameworkCore;
using Zefir.BL.Abstractions;
using Zefir.BL.Contracts;
using Zefir.Core.Errors;
using Zefir.DAL;

namespace Zefir.BL.Services;

public class BasketService : IBasketService
{
    private readonly AppDbContext _appDbContext;

    public BasketService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<PublicBasketData> GetUsersBasket(int userId)
    {
        var basket = await _appDbContext.Baskets
            .Include(b => b.Products)
            .Include(basket => basket.User)
            .FirstOrDefaultAsync(b => b.User.Id == userId);
        if (basket is null) throw new ServiceNotFoundError("No such user");
        return new PublicBasketData(basket.User.Id, basket.Products);
    }

    public async Task AddProductToBasket(int userId, int productId)
    {
        var candidate = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (candidate is null) throw new ServiceNotFoundError("No such product");

        var basket = await _appDbContext.Baskets
            .Include(basket => basket.Products)
            .FirstOrDefaultAsync(b => b.User.Id == userId);
        if (basket is null) throw new ServiceNotFoundError("No such basket");

        if (!basket.Products.Contains(candidate)) basket.Products.Add(candidate);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task RemoveProductFromBasket(int userId, int productId)
    {
        var candidate = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (candidate is null) throw new ServiceNotFoundError("No such product");

        var basket = await _appDbContext.Baskets
            .Include(basket => basket.Products)
            .FirstOrDefaultAsync(b => b.User.Id == userId);
        if (basket is null) throw new ServiceNotFoundError("No such basket");

        if (basket.Products.Contains(candidate)) basket.Products.Remove(candidate);
        await _appDbContext.SaveChangesAsync();
    }
}
