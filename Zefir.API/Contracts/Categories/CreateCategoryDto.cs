namespace Zefir.API.Contracts.Categories;

/// <summary>
///     Contract for creating new category
/// </summary>
/// <param name="Name">Category name</param>
/// <param name="Description">Category description</param>
public record CreateCategoryDto(string Name, string Description);
