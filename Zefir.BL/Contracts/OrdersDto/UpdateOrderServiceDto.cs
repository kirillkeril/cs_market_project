namespace Zefir.BL.Contracts.OrdersDto;

/// <summary>
///     -1 = Fail 0 = Default (processing) 1 = InWork 2 = Done <see cref="Status" />
/// </summary>
/// <param name="Status">Status <see cref="Status" /></param>
public record UpdateOrderServiceDto(int Status);
