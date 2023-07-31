using Zefir.Domain.Entity;

namespace Zefir.DAL.Dto;

public record CreateOrderDto(
    int UserId,
    int[] ProductsId,
    string Deadline
);

public record PublicOrderData(
    int Id,
    int UserId,
    IEnumerable<Product> Products,
    int Status,
    DateOnly Deadline
);
