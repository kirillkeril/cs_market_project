using Zefir.Core.Entity;

namespace Zefir.BL.Contracts;

public record ServiceCreateOrderDto(
    int[] ProductsId,
    string Deadline
);

/// <summary>
///     -1 = Fail 0 = Default (processing) 1 = InWork 2 = Done <see cref="Status" />
/// </summary>
/// <param name="Status">Status <see cref="Status" /></param>
public record ServiceUpdateOrderDto(int Status);

public record PublicOrderData(
    int Id,
    int UserId,
    IEnumerable<Product> Products,
    string Status,
    DateOnly Deadline,
    double Sum
);
