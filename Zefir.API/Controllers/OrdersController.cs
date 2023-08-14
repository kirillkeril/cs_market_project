using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zefir.API.Contracts.Orders;
using Zefir.BL.Abstractions;
using Zefir.BL.Contracts.OrdersDto;
using Zefir.Core.Entity;

namespace Zefir.API.Controllers;

/// <summary>
///     Operations with Order entity (authorize only)
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private const string GetAllOrderRouteName = "get-all-orders";
    private const string GetOwnOrdersRouteName = "get-own-orders";
    private const string CreateOrderRouteName = "create-order";
    private const string UpdateOrderStatus = "update-order-status";

    /// <summary>
    /// </summary>
    /// <param name="orderService"></param>
    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    ///     Gets all orders or all orders for one user by id (admin only)
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    [HttpGet("all", Name = GetAllOrderRouteName)]
    [Authorize(Roles = Role.AdminRole)]
    public async Task<IActionResult> GetAllOrders([FromQuery] int? userId = null)
    {
        var result = await _orderService.GetAllOrders(userId);
        return Ok(result);
    }

    /// <summary>
    ///     Get user's own orders by his claim
    /// </summary>
    /// <returns></returns>
    [HttpGet("", Name = GetOwnOrdersRouteName)]
    [Authorize]
    public async Task<IActionResult> GetOwnOrders()
    {
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is not null && int.TryParse(userIdClaim.Value, out var userId))
        {
            var result = await _orderService.GetOwnOrders(userId);
            return Ok(result);
        }

        return Unauthorized();
    }

    /// <summary>
    /// User create order with product
    /// </summary>
    /// <param name="dto">Data<see cref="Contracts.Orders.CreateOrderDto"/></param>
    /// <returns></returns>
    [HttpPost("", Name = CreateOrderRouteName)]
    [Authorize]
    public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
    {
        var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userIdClaim is null) return Unauthorized();

        var userId = int.Parse(userIdClaim.Value);
        var serviceDto = new CreateOrderServiceDto(dto.ProductsId, dto.Deadline);
        var newOrder = await _orderService.CreateOrder(userId, serviceDto);
        return CreatedAtRoute(CreateOrderRouteName, new { newOrder.Id }, newOrder);
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    [HttpPut("{id:int}", Name = UpdateOrderStatus)]
    [Authorize(Roles = Role.AdminRole)]
    public async Task<IActionResult> UpdateOrder(int id, UpdateOrderDto dto)
    {
        var serviceDto = new UpdateOrderServiceDto(dto.Status);
        var result = await _orderService.UpdateOrderStatus(id, serviceDto);
        return Ok(result);
    }
}
