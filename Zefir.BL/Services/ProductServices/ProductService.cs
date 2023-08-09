using Microsoft.EntityFrameworkCore;
using Zefir.BL.Abstractions;
using Zefir.BL.Contracts.ProductsDto;
using Zefir.Common.Errors;
using Zefir.Common.Helpers;
using Zefir.Core.Entity;
using Zefir.DAL;

namespace Zefir.BL.Services.ProductServices;

public class ProductService : IProductService
{
    private readonly AppDbContext _appDbContext;
    private readonly PaginationHelper _pagination = new(10);
    private readonly ISortProductsService _sortProductsService;
    private readonly FilterProductsService _filterProductsService;

    public ProductService(AppDbContext appDbContext, ISortProductsService sortProductsService,
        FilterProductsService filterProductsService)
    {
        _appDbContext = appDbContext;
        _sortProductsService = sortProductsService;
        _filterProductsService = filterProductsService;
    }

    public async Task<ProductsPagesServiceDto> GetAllProducts(int page = 0, string search = "", string sortBy = "",
        string category = "", string thematic = "")
    {
        // Remove spaces for search
        search = search.Replace(" ", "");
        sortBy = sortBy.Replace(" ", "");

        //search and filter products
        var products = _appDbContext.Products.Where(
            p =>
                p.Category.Name.Contains(category) &&
                p.Characteristics != null && (p.Name.Contains(search) ||
                                              p.Description.Contains(search) ||
                                              p.Characteristics.Any(c => c.Value.Contains(search)))
        );

        var filteredByThematicProducts = await _filterProductsService.FilterByThematic(products, thematic);
        var filteredByCategory = await _filterProductsService.FilterByCategory(filteredByThematicProducts, thematic);

        var totalPages = _pagination.ComputeCountOfPages(filteredByCategory.Count());
        var pagedProducts = _pagination.GetPagedItems(filteredByCategory, page);

        var productsData = await pagedProducts
            .Include(p => p.Category)
            .Include(p => p.Characteristics)
            .ToListAsync();

        //Creates public contract
        var publicProductData = new List<ProductInfoServiceDto>();
        foreach (var p in productsData)
        {
            var characteristics = new Dictionary<string, string>();
            if (p.Characteristics != null)
                foreach (var c in p.Characteristics)
                    characteristics.Add(c.Key, c.Value);
            var productData =
                new ProductInfoServiceDto(p.Id, p.Name, p.Description, p.Category.Name, p.Price, characteristics,
                    p.CreatedAt);
            publicProductData.Add(productData);
        }

        if (!string.IsNullOrWhiteSpace(sortBy))
            publicProductData = await _sortProductsService.SortProducts(publicProductData, sortBy);

        var productDto = new ProductsPagesServiceDto(publicProductData, totalPages, page);

        return productDto;
    }


    public async Task<Product?> GetProductById(int id)
    {
        return await _appDbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<ProductInfoServiceDto?> CreateProduct(CreateProductServiceDto dto)
    {
        //TODO make image file path
        var newProduct = new Product(dto.Name, dto.Description, dto.Price, "");
        try
        {
            foreach (var characteristic in dto.Characteristics)
            {
                var newCharacteristic = new Characteristics(characteristic.Key, characteristic.Value);
                newProduct.Characteristics?.Add(newCharacteristic);
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
        await _appDbContext.Characteristics.AddRangeAsync(newProduct.Characteristics ??
                                                          throw new InvalidOperationException());
        await _appDbContext.SaveChangesAsync();

        var characteristics = new Dictionary<string, string>();
        foreach (var characteristic in newProduct.Characteristics)
            characteristics.Add(characteristic.Key, characteristic.Value);

        var publicProductData = new ProductInfoServiceDto(newProduct.Id, newProduct.Name, newProduct.Description,
            newProduct.Category.Name, dto.Price, characteristics, newProduct.CreatedAt);
        return publicProductData;
    }

    public async Task<ProductInfoServiceDto> UpdateProduct(int id, UpdateProductServiceDto dto)
    {
        var product = await _appDbContext.Products.Include(product => product.Characteristics).FirstOrDefaultAsync(p => p.Id == id);
        if (product is null) throw new ServiceNotFoundError("Product not found");
        product.Name = dto.Name;
        product.Description = dto.Description;
        foreach (var characteristic in dto.Characteristics)
        {
            var newCharacteristic = new Characteristics(characteristic.Key, characteristic.Value);
            var possibleCharacteristic = product.Characteristics?.Find(c => c.Key == newCharacteristic.Key);

            if (possibleCharacteristic is not null) possibleCharacteristic.Value = newCharacteristic.Value;
            else product.Characteristics?.Add(newCharacteristic);
        }

        var targetCategory = await _appDbContext.Categories.FirstOrDefaultAsync(c =>
            c.Name.Equals(dto.CategoryName));
        if (targetCategory is null) throw new ServiceBadRequestError((nameof(dto.CategoryName), "No such category"));
        product.Category = targetCategory;

        await _appDbContext.SaveChangesAsync();

        var characteristics = new Dictionary<string, string>();
        if (product.Characteristics != null)
            foreach (var characteristic in product.Characteristics)
                characteristics.Add(characteristic.Key, characteristic.Value);
        var publicProductData = new ProductInfoServiceDto(product.Id, product.Name, product.Description,
            product.Category.Name,
            product.Price,
            characteristics, product.CreatedAt);

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
