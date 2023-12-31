using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zefir.API.Contracts.Products;
using Zefir.BL.Abstractions;
using Zefir.BL.Contracts.CategoryDto;
using Zefir.BL.Contracts.ProductsDto;
using Zefir.Core.Entity;

namespace Zefir.API.Controllers;

/// <summary>
///  CRUD operations with products (admin only)
/// </summary>
[Authorize(Roles = Role.AdminRole)]
[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private const string GetAllRouteName = "get-products";
    private const string GetByIdRouteName = "get-products-by-id";
    private const string CreateProductRouteName = "create-product";
    private const string UpdateProductRouteName = "update-product";
    private const string DeleteProductRouteName = "delete-product";

    /// <summary>
    /// </summary>
    /// <param name="productService"></param>
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    ///     Get all products by pages
    /// </summary>
    /// <param name="page">Number of page</param>
    /// <param name="searchQuery">Optional search query</param>
    /// <param name="sortBy">Expected values: "name", "price", "date", "category", default - "price"</param>
    [HttpGet(Name = GetAllRouteName)]
    [AllowAnonymous]
    public async Task<ActionResult> GetAll(int page, string searchQuery = "", string sortBy = "")
    {
        var products = await _productService.GetAllProducts(page, searchQuery, sortBy);

        var totalPages = 0;
        HttpContext.Response.Headers.Add("X-Total-Count", totalPages.ToString());
        return Ok(products);
    }

    /// <summary>
    ///     Get one product by id
    /// </summary>
    /// <param name="id">integer id</param>
    /// <returns>One product or null</returns>
    [HttpGet("{id}", Name = GetByIdRouteName)]
    [AllowAnonymous]
    public async Task<ActionResult> GetById(int id)
    {
        var product = await _productService.GetProductById(id);
        if (product is null) return NotFound();
        return Ok(product);
    }

    /// <summary>
    ///     Creates new product (admin only)
    /// </summary>
    /// <param name="dto">Data <see cref="CreateCategoryServiceDto"/></param>
    [HttpPost(Name = CreateProductRouteName)]
    // [Authorize(Roles = Role.AdminRole)]
    [AllowAnonymous]
    public async Task<ActionResult> CreateProduct(CreateProductDto dto)
    {
        var serviceContract = new CreateProductServiceDto(dto.Name, dto.Description, dto.CategoryName, dto.Price,
            dto.Characteristics);
        var newProduct = await _productService.CreateProduct(serviceContract);
        if (newProduct != null) return CreatedAtRoute(GetByIdRouteName, new { newProduct.Id }, newProduct);

        return BadRequest(new { errpor = "Product not created. Check your data." });
    }

    /// <summary>
    ///     Updates product by id (admin only)
    /// </summary>
    /// <param name="id">int id</param>
    /// <param name="dto">Data to update</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id:int}", Name = UpdateProductRouteName)]
    [Authorize(Roles = Role.AdminRole)]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto dto)
    {
        var serviceContract = new UpdateProductServiceDto(dto.Name, dto.Description, dto.CategoryName, dto.Price,
            dto
                .Characteristics);
        var updatedProduct = await _productService.UpdateProduct(id, serviceContract);
        return Ok(new { result = updatedProduct });
    }

    /// <summary>
    /// Delete product by id (admin only)
    /// </summary>
    /// <param name="id">integer id</param>
    /// <returns>Ok OR 404 OR 500 with errors</returns>
    [HttpDelete("{id:int}", Name = DeleteProductRouteName)]
    [Authorize(Roles = Role.AdminRole)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProduct(id);
        if (result) return Ok();
        return NotFound(new { });
    }
}
