using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zefir.API.Contracts.Categories;
using Zefir.BL.Abstractions;
using Zefir.BL.Contracts.CategoryDto;
using Zefir.Core.Entity;

namespace Zefir.API.Controllers;

/// <summary>
///     CRUD operations with categories
/// </summary>
[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;
    private const string GetAllRouteName = "get-all";
    private const string GetByNameRouteName = "get-by-name";
    private const string CreateNewRouteName = "create-new";
    private const string UpdateNewRouteName = "update";
    private const string DeleteRouteName = "delete";

    /// <summary>
    /// </summary>
    /// <param name="categoryService"></param>
    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// Returns new list of categories
    /// </summary>
    /// <returns>List of categories</returns>
    [HttpGet("", Name = GetAllRouteName)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllCategories();
        return Ok(new { categories = result });
    }

    /// <summary>
    /// Find category by name
    /// </summary>
    /// <param name="name">String name of category</param>
    /// <returns>One category object</returns>
    [HttpGet("{name}", Name = GetByNameRouteName)]
    public async Task<IActionResult> GetByName(string name)
    {
        var result = await _categoryService.GetCategoryByName(name);
        return Ok(result);
    }

    /// <summary>
    /// Creates new category (admin only)
    /// </summary>
    /// <param name="dto">Data <see cref="CreateCategoryServiceDto"/></param>
    /// <returns>201 with created at or 400 with errors or 500 with errors</returns>
    [HttpPost("", Name = CreateNewRouteName)]
    [Authorize(Roles = Role.AdminRole)]
    public async Task<IActionResult> Create(CreateCategoryServiceDto dto)
    {
        var result = await _categoryService.CreateNewCategory(dto);
        return CreatedAtRoute(GetByNameRouteName, new { result.Name }, result);
    }

    /// <summary>
    /// Updates category (admin only)
    /// </summary>
    /// <param name="name">string name of category</param>
    /// <param name="dto">Data <see cref="CreateCategoryServiceDto"/></param>
    /// <returns>200 with updated object or 400 with errors or 404 with errors or 500 with errors</returns>
    [HttpPut("{name}", Name = UpdateNewRouteName)]
    [Authorize(Roles = Role.AdminRole)]
    public async Task<IActionResult> Update(string name, UpdateCategoryDto dto)
    {
        var serviceContract = new UpdateCategoryServiceDto(dto.Name, dto.Description);
        var result = await _categoryService.UpdateCategory(name, serviceContract);
        return Ok(result);
    }

    /// <summary>
    /// Delete category by name (admin only)
    /// </summary>
    /// <param name="name">string name</param>
    /// <returns>204 no content OR 404 OR 500 with errors</returns>
    [HttpDelete("{name}", Name = DeleteRouteName)]
    [Authorize(Roles = Role.AdminRole)]
    public async Task<IActionResult> Delete(string name)
    {
        var result = await _categoryService.DeleteCategory(name);
        if (result) return NoContent();
        return NotFound(new { });
    }
}
