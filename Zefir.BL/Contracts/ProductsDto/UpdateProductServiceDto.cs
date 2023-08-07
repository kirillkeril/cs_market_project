namespace Zefir.BL.Contracts.ProductsDto;

public record UpdateProductServiceDto
(
    string Name,
    string Description,
    string CategoryName,
    double Price,
    Dictionary<string, string> Characteristics
);
