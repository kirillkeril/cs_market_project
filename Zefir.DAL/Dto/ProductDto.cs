namespace Zefir.DAL.Dto;

public record CreateProductDto
(
    string Name,
    string Description,
    Dictionary<string, string> Characteristics
);

public record UpdateProductDto
(
    string Name,
    string Description,
    Dictionary<string, string> Characteristics
);
