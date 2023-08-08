using Zefir.BL.Abstractions;
using Zefir.BL.Contracts.ProductsDto;

namespace Zefir.BL.Services.ProductServices;

public class SortProductsService : ISortProductsService
{
    public async Task<List<ProductInfoServiceDto>> SortProducts
        (List<ProductInfoServiceDto> products, string sortBy)
    {
        List<ProductInfoServiceDto> sortedProducts;
        switch (sortBy.ToLower())
        {
            case "name":
                sortedProducts = products.OrderBy(p => p.Name).ToList();
                break;
            case "category":
                sortedProducts = products.OrderBy(p => p.CategoryName).ToList();
                break;
            case "price":
                sortedProducts = products.OrderBy(p => p.Price).ToList();
                break;
            case "date":
                sortedProducts = products.OrderBy(p => p.CreatedAt).ToList();
                break;
            default:
                sortedProducts = products.OrderBy(p => p.Price).ToList();
                break;
        }

        return sortedProducts;
    }
}
