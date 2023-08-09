using Zefir.BL.Contracts.ProductsDto;
using Zefir.Core.Entity;

namespace Zefir.BL.Abstractions;

public interface IProductService
{
    Task<ProductsPagesServiceDto> GetAllProducts(int page = 0, string search = "", string sortBy = "",
        string category = "");
    Task<Product?> GetProductById(int id);
    Task<ProductInfoServiceDto?> CreateProduct(CreateProductServiceDto dto);
    Task<ProductInfoServiceDto> UpdateProduct(int id, UpdateProductServiceDto dto);
    Task<bool> DeleteProduct(int id);
}
