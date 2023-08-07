namespace Zefir.BL.Contracts.OrdersDto;

public record CreateOrderServiceDto(
    int[] ProductsId,
    string Deadline
);