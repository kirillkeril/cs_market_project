using Zefir.BL.Contracts;

namespace Zefir.BL.Abstractions;

public interface IBasketService
{
    Task<PublicBasketData> GetUsersBasket(int userId);
    Task AddProductToBasket(int userId, int productId);

    Task RemoveProductFromBasket(int userId, int productId);
}
