namespace Zefir.DAL.Dto;

public record CreateProductDto
(
    string Name,
    string Description,
    string CategoryName,
    Dictionary<string, string> Characteristics
);

public record UpdateProductDto
(
    string Name,
    string Description,
    string CategoryName,
    Dictionary<string, string> Characteristics
);

public record PublicProductData(int Id, string Name, string Description, string CategoryName, Dictionary<string,
    string> Characteristics);
