using Microsoft.EntityFrameworkCore;
using Zefir.BL.Contracts;
using Zefir.Common.Helpers;
using Zefir.Core.Entity;
using Zefir.Core.Errors;
using Zefir.DAL;

namespace Zefir.BL.Services;

public class ProductService
{
    private readonly AppDbContext _appDbContext;
    private readonly PaginationHelper _pagination;

    public ProductService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
        _pagination = new PaginationHelper(10);
    }

    public async Task<GetAllProductsDto> GetAllProducts(int page = 0, string search = "")
    {
        var products = _appDbContext.Products.Where(
            p =>
                p.Name.Contains(search) ||
                p.Description.Contains(search) ||
                p.Category.Description.Contains(search) ||
                p.Category.Name.Contains(search) ||
                p.Characteristics.Any(c => c.Value.Contains(search))
        );

        var totalPages = _pagination.ComputeCountOfPages(products.Count());
        var pagedProducts = _pagination.GetPagedItems(products, page);

        var productsData = await pagedProducts
            .Include(p => p.Category)
            .Include(p => p.Characteristics)
            .ToListAsync();

        var publicProductData = new List<PublicProductData>();
        foreach (var p in productsData)
        {
            var characteristics = new Dictionary<string, string>();
            foreach (var c in p.Characteristics) characteristics.Add(c.Key, c.Value);
            var productData =
                new PublicProductData(p.Id, p.Name, p.Description, p.Category.Name, p.Price, characteristics);
            publicProductData.Add(productData);
        }

        var productDto = new GetAllProductsDto(publicProductData, totalPages, page);

        return productDto;
    }


    public async Task<Product?> GetProductById(int id)
    {
        return await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PublicProductData?> CreateProduct(ServiceCreateProductDto dto)
    {
        //TODO make image file path
        var newProduct = new Product(dto.Name, dto.Description, dto.Price, "");
        try
        {
            foreach (var characteristic in dto.Characteristics)
            {
                var newCharacteristic = new Characteristics(characteristic.Key, characteristic.Value);
                newProduct.Characteristics.Add(newCharacteristic);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }

        var targetCategory = await _appDbContext.Categories.FirstOrDefaultAsync(c =>
            c.Name.Equals(dto.CategoryName));
        if (targetCategory is null) throw new ServiceBadRequestError((nameof(dto.CategoryName), "No such category"));
        newProduct.Category = targetCategory;

        _appDbContext.Products.Add(newProduct);
        await _appDbContext.Characteristics.AddRangeAsync(newProduct.Characteristics);
        await _appDbContext.SaveChangesAsync();

        var characteristics = new Dictionary<string, string>();
        foreach (var characteristic in newProduct.Characteristics)
            characteristics.Add(characteristic.Key, characteristic.Value);

        var publicProductData = new PublicProductData(newProduct.Id, newProduct.Name, newProduct.Description,
            newProduct.Category.Name, dto.Price, characteristics);
        return publicProductData;
    }

    public async Task<PublicProductData> UpdateProduct(int id, ServiceUpdateProductDto dto)
    {
        var product = await _appDbContext.Products.Include(product => product.Characteristics).FirstOrDefaultAsync(p => p.Id == id);
        if (product is null) throw new ServiceNotFoundError("Product not found");
        product.Name = dto.Name;
        product.Description = dto.Description;
        foreach (var characteristic in dto.Characteristics)
        {
            var newCharacteristic = new Characteristics(characteristic.Key, characteristic.Value);
            var possibleCharacteristic = product.Characteristics.Find(c => c.Key == newCharacteristic.Key);

            if (possibleCharacteristic is not null) possibleCharacteristic.Value = newCharacteristic.Value;
            else product.Characteristics.Add(newCharacteristic);
        }

        var targetCategory = await _appDbContext.Categories.FirstOrDefaultAsync(c =>
            c.Name.Equals(dto.CategoryName));
        if (targetCategory is null) throw new ServiceBadRequestError((nameof(dto.CategoryName), "No such category"));
        product.Category = targetCategory;

        await _appDbContext.SaveChangesAsync();

        var characteristics = new Dictionary<string, string>();
        foreach (var characteristic in product.Characteristics)
            characteristics.Add(characteristic.Key, characteristic.Value);
        var publicProductData = new PublicProductData(product.Id, product.Name, product.Description,
            product.Category.Name,
            product.Price,
            characteristics);

        return publicProductData;
    }

    public async Task<bool> DeleteProduct(int id)
    {
        var candidate = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (candidate is null) return false;
        _appDbContext.Products.Remove(candidate);
        await _appDbContext.SaveChangesAsync();
        return true;
    }
}
