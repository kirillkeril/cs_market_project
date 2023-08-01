using Microsoft.EntityFrameworkCore;
using Zefir.Core.Entity;
using Zefir.DAL.Dto;
using Zefir.DAL.Errors;

namespace Zefir.DAL.Services;

public class ProductService
{
    private readonly AppDbContext _appDbContext;

    public ProductService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<List<PublicProductData>> GetAllProducts()
    {
        var products = await _appDbContext.Products
            .Include(p => p.Characteristics)
            .Include(p => p.Category)
            .ToListAsync();

        var publicProductData = new List<PublicProductData>();
        foreach (var p in products)
        {
            var characteristics = new Dictionary<string, string>();
            foreach (var c in p.Characteristics) characteristics.Add(c.Key, c.Value);
            var productData = new PublicProductData(p.Id, p.Name, p.Description, p.Category.Name, characteristics);
            publicProductData.Add(productData);
        }

        return publicProductData;
    }

    public async Task<List<PublicProductData>> GetBySearch(string search)
    {
        var products = _appDbContext.Products.Where(
            p =>
                p.Name.Contains(search) ||
                p.Description.Contains(search) ||
                p.Category.Description.Contains(search) ||
                p.Category.Name.Contains(search) ||
                p.Characteristics.Any(c => c.Value.Contains(search)));

        var productsData = await products
            .Include(p => p.Category)
            .Include(p => p.Characteristics)
            .ToListAsync();

        var publicProductData = new List<PublicProductData>();
        foreach (var p in productsData)
        {
            var characteristics = new Dictionary<string, string>();
            foreach (var c in p.Characteristics) characteristics.Add(c.Key, c.Value);
            var productData = new PublicProductData(p.Id, p.Name, p.Description, p.Category.Name, characteristics);
            publicProductData.Add(productData);
        }

        return publicProductData;
    }

    public async Task<Product?> GetProductById(int id)
    {
        return await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<PublicProductData?> CreateProduct(CreateProductDto dto)
    {
        //TODO make image file path
        var newProduct = new Product(dto.Name, dto.Description, "");
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
            newProduct.Category.Name, characteristics);
        return publicProductData;
    }

    public async Task<PublicProductData> UpdateProduct(int id, UpdateProductDto dto)
    {
        var product = await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
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
