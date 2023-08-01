namespace Zefir.API.Contracts.Orders;

/// <summary>
///     Contract for create new order
/// </summary>
/// <param name="ProductsId">List of integer ids of products</param>
/// <param name="Deadline">Date deadline</param>
public record CreateOrderDto(
    int[] ProductsId,
    string Deadline
);
