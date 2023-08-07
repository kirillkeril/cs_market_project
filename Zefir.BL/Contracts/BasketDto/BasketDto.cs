using Zefir.Core.Entity;

namespace Zefir.BL.Contracts.BasketDto;

public record BasketInfoServiceDto(int UserId, List<Product> Products);
