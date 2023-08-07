using Zefir.Core.Entity;

namespace Zefir.BL.Contracts.OrdersDto;

public record OrderInfoServiceDto(
    int Id,
    int UserId,
    IEnumerable<Product> Products,
    string Status,
    DateOnly Deadline,
    double Sum
);
