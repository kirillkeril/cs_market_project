using Zefir.BL.Contracts.OrdersDto;

namespace Zefir.BL.Abstractions;

public interface IOrderService
{
    Task<List<OrderInfoServiceDto>> GetAllOrders(int? userId = null);
    Task<List<OrderInfoServiceDto>> GetOwnOrders(int ownerId);
    Task<OrderInfoServiceDto> CreateOrder(int userId, CreateOrderServiceDto orderDto);
    Task<OrderInfoServiceDto> UpdateOrderStatus(int id, UpdateOrderServiceDto dto);
}