namespace Zefir.API.Contracts.Products;

/// <summary>
///     Contract for create new product
/// </summary>
/// <param name="Name">Product name</param>
/// <param name="Description">Product description</param>
/// <param name="CategoryName">Category (must exists)</param>
/// <param name="Price">Double price in Rubles</param>
/// <param name="Characteristics">Object with characteristics <see cref="Characteristics" /></param>
public record CreateProductDto
(
    string Name,
    string Description,
    string CategoryName,
    double Price,
    Dictionary<string, string> Characteristics
);
