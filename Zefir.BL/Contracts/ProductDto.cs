namespace Zefir.BL.Contracts;

public record ServiceCreateProductDto
(
    string Name,
    string Description,
    string CategoryName,
    double Price,
    Dictionary<string, string> Characteristics
);

public record ServiceUpdateProductDto
(
    string Name,
    string Description,
    string CategoryName,
    double Price,
    Dictionary<string, string> Characteristics
);

public record PublicProductData(int Id, string Name, string Description, string CategoryName, double Price,
    Dictionary<string,
    string> Characteristics);

public record GetAllProductsDto(List<PublicProductData> ProductData, int TotalPages, int CurrentPage);
