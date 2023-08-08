namespace Zefir.BL.Contracts.ProductsDto;

public record ProductInfoServiceDto(
    int Id,
    string Name,
    string Description,
    string CategoryName,
    double Price,
    Dictionary<string, string> Characteristics,
    DateTime CreatedAt);
