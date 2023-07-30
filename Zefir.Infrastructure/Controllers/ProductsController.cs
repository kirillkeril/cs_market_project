using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zefir.DAL.Dto;
using Zefir.DAL.Services;
using Zefir.Domain.Entity;

namespace Zefir.Infrastructure.Controllers;

/// <summary>
///  CRUD operations with products (admin only)
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly ProductService _productService;
    private const string GetAllRouteName = "get-products";
    private const string GetByIdRouteName = "get-products-by-id";
    private const string CreateProductRouteName = "create-product";
    private const string UpdateProductRouteName = "update-product";
    private const string DeleteProductRouteName = "delete-product";

    /// <summary>
    /// </summary>
    /// <param name="productService"></param>
    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    ///     Get all products as IQueryable to implement search
    /// </summary>
    /// <param name="searchQuery">Optional search query</param>
    [HttpGet(Name = GetAllRouteName)]
    [AllowAnonymous]
    public async Task<ActionResult> GetAll(string? searchQuery = "")
    {
        List<Product> products;

        if (string.IsNullOrWhiteSpace(searchQuery))
            products = await _productService.GetAllProducts();
        else products = await _productService.GetBySearch(searchQuery);

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
    /// <param name="dto">Data <see cref="CreateCategoryDto"/></param>
    [HttpPost(Name = CreateProductRouteName)]
    public async Task<ActionResult> CreateProduct(CreateProductDto dto)
    {
        try
        {
            var newProduct = await _productService.CreateProduct(dto);
            if (newProduct != null)
                return CreatedAtRoute(GetByIdRouteName, new { newProduct.Id }, newProduct);
            return BadRequest(new { errpor = "Product not created. Check your data." });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
    }

    /// <summary>
    ///     Updates product by id (admin only)
    /// </summary>
    /// <param name="id">int id</param>
    /// <param name="dto">Data to update</param>
    /// <returns>Updated product</returns>
    [HttpPut("{id:int}", Name = UpdateProductRouteName)]
    public async Task<IActionResult> UpdateProduct(int id, UpdateProductDto dto)
    {
        try
        {
            var updatedProduct = await _productService.UpdateProduct(id, dto);
            if (updatedProduct is not null) return Ok(new { result = updatedProduct });
            return BadRequest(new { errors = new List<string> { "Invalid data" } });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
    }

    /// <summary>
    /// Delete product by id (admin only)
    /// </summary>
    /// <param name="id">integer id</param>
    /// <returns>Ok OR 404 OR 500 with errors</returns>
    [HttpDelete("{id:int}", Name = DeleteProductRouteName)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        try
        {
            var result = await _productService.DeleteProduct(id);
            if (result) return Ok();
            return NotFound(new { });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
    }
}
