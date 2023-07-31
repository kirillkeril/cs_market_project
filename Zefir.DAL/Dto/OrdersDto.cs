using Zefir.Domain.Entity;

namespace Zefir.DAL.Dto;

public record CreateOrderDto(
    int[] ProductsId,
    string Deadline
);

/// <summary>
///     -1 = Fail 0 = Default (processing) 1 = InWork 2 = Done <see cref="Status" />
/// </summary>
/// <param name="Status">Status <see cref="Status" /></param>
public record UpdateOrderStatusDto(int Status);

public record PublicOrderData(
    int Id,
    int UserId,
    IEnumerable<Product> Products,
    string Status,
    DateOnly Deadline
);
