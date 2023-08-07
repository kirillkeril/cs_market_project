namespace Zefir.BL.Contracts.ProductsDto;

public record ProductsPagesServiceDto(List<ProductInfoServiceDto> ProductData, int TotalPages, int CurrentPage);