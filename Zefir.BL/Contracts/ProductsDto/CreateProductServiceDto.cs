namespace Zefir.BL.Contracts.ProductsDto;

public record CreateProductServiceDto
(
    string Name,
    string Description,
    string CategoryName,
    double Price,
    Dictionary<string, string> Characteristics
);