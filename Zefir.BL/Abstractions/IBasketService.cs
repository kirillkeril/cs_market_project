using Zefir.BL.Contracts.BasketDto;

namespace Zefir.BL.Abstractions;

public interface IBasketService
{
    Task<BasketInfoServiceDto> GetUsersBasket(int userId);
    Task AddProductToBasket(int userId, int productId);

    Task RemoveProductFromBasket(int userId, int productId);
}
