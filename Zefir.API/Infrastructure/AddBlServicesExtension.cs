using Zefir.BL.Abstractions;
using Zefir.BL.Services;
using Zefir.BL.Services.ProductServices;

namespace Zefir.API.Infrastructure;

/// <summary>
/// </summary>
public static class AddBlServicesExtension
{
    /// <summary>
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder UseBlServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ITokenService>(new TokenService(builder.Configuration));

        builder.Services.AddTransient<ISortProductsService, SortProductsService>();
        builder.Services.AddTransient<IProductService, ProductService>();
        builder.Services.AddTransient<SortProductsService>();
        builder.Services.AddTransient<ICategoryService, CategoryService>();
        builder.Services.AddTransient<IOrderService, OrderService>();
        builder.Services.AddTransient<IBasketService, BasketService>();
        builder.Services.AddTransient<IAccountService, AccountService>();
        return builder;
    }
}
