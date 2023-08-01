namespace Zefir.API.Contracts.Orders;

/// <summary>
///     Contract for update Order's data
/// </summary>
/// <param name="Status">String status <see cref="Status" /></param>
public record UpdateOrderDto(int Status);