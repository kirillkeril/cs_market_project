using Microsoft.AspNetCore.Mvc;
using Zefir.DAL.Dto;
using Zefir.DAL.Errors;
using Zefir.DAL.Services;

namespace Zefir.Infrastructure.Controllers;

/// <summary>
/// </summary>
[ApiController]
[Route("[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _categoryService;
    private const string GetAllRouteName = "get-all";
    private const string GetByNameRouteName = "get-by-name";
    private const string CreateNewRouteName = "create-new";

    // TODO make documentation
    /// <summary>
    /// </summary>
    /// <param name="categoryService"></param>
    public CategoriesController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [HttpGet("", Name = GetAllRouteName)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _categoryService.GetAllCategories();
        return Ok(new { categories = result });
    }

    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
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
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("", Name = CreateNewRouteName)]
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
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPut("{name}")]
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
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    [HttpDelete("{name}")]
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
