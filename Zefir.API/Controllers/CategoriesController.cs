using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zefir.DAL.Dto;
using Zefir.DAL.Errors;
using Zefir.DAL.Services;
using Zefir.Core.Entity;

namespace Zefir.API.Controllers;

/// <summary>
///     CRUD operations with categories
/// </summary>
[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _categoryService;
    private const string GetAllRouteName = "get-all";
    private const string GetByNameRouteName = "get-by-name";
    private const string CreateNewRouteName = "create-new";
    private const string UpdateNewRouteName = "update";
    private const string DeleteRouteName = "delete";

    /// <summary>
    /// </summary>
    /// <param name="categoryService"></param>
    public CategoriesController(CategoryService categoryService)
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
        var errors = new List<string>();
        try
        {
            var result = await _categoryService.GetCategoryByName(name);
            return Ok(result);
        }
        catch (ServiceNotFoundError e)
        {
            errors.Add(e.Message);
            return NotFound(new { errors });
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return StatusCode(500, new { errors });
        }
    }

    /// <summary>
    /// Creates new category (admin only)
    /// </summary>
    /// <param name="dto">Data <see cref="CreateCategoryDto"/></param>
    /// <returns>201 with created at or 400 with errors or 500 with errors</returns>
    [HttpPost("", Name = CreateNewRouteName)]
    [Authorize(Roles = Role.AdminRole)]
    public async Task<IActionResult> Create(CreateCategoryDto dto)
    {
        try
        {
            var result = await _categoryService.CreateNewCategory(dto);
            return CreatedAtRoute(GetByNameRouteName, new { result.Name }, result);
        }
        catch (ServiceBadRequestError e)
        {
            return BadRequest(new { errors = e.FieldErrors });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
    }

    /// <summary>
    /// Updates category (admin only)
    /// </summary>
    /// <param name="name">string name of category</param>
    /// <param name="dto">Data <see cref="CreateCategoryDto"/></param>
    /// <returns>200 with updated object or 400 with errors or 404 with errors or 500 with errors</returns>
    [HttpPut("{name}", Name = UpdateNewRouteName)]
    [Authorize(Roles = Role.AdminRole)]
    public async Task<IActionResult> Update(string name, CreateCategoryDto dto)
    {
        try
        {
            var result = await _categoryService.UpdateCategory(name, dto);
            return Ok(result);
        }
        catch (ServiceNotFoundError e)
        {
            return NotFound(new { errors = new List<string> { e.Message } });
        }
        catch (ServiceBadRequestError e)
        {
            return BadRequest(new { errors = e.FieldErrors });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
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
        try
        {
            var result = await _categoryService.DeleteCategory(name);
            if (result) return NoContent();
            return NotFound(new { });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
    }
}
