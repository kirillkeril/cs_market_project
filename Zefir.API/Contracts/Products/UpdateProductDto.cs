namespace Zefir.API.Contracts.Products;

/// <summary>
///     Contract for update info about product
/// </summary>
/// <param name="Name">Product name</param>
/// <param name="Description">Product description</param>
/// <param name="CategoryName">Category name (must exists)</param>
/// <param name="Price">Double price in rubles</param>
/// <param name="Characteristics">Object with characteristics <see cref="Characteristics" /></param>
public record UpdateProductDto
(
    string Name,
    string Description,
    string CategoryName,
    double Price,
    Dictionary<string, string> Characteristics
);
