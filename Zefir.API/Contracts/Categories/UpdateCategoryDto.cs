namespace Zefir.API.Contracts.Categories;

/// <summary>
///     Contract for update category
/// </summary>
/// <param name="Name">category name</param>
/// <param name="Description">category description</param>
public record UpdateCategoryDto(string Name, string Description);
