using Zefir.BL.Contracts.ProductsDto;

namespace Zefir.BL.Abstractions;

public interface ISortProductsService
{
    Task<List<ProductInfoServiceDto>> SortProducts(List<ProductInfoServiceDto> products, string sortBy);
}