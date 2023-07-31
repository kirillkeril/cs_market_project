using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zefir.DAL.Dto;
using Zefir.DAL.Errors;
using Zefir.DAL.Services;

namespace Zefir.Infrastructure.Controllers;

/// <summary>
///     Operations with Order entity (authorize only)
/// </summary>
[ApiController]
[Route("[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    private const string CreateOrderRouteName = "create-order";

    /// <summary>
    /// </summary>
    /// <param name="orderService"></param>
    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [HttpPost("", Name = CreateOrderRouteName)]
    public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
    {
        try
        {
            var newOrder = await _orderService.CreateOrder(dto);
            return CreatedAtRoute(CreateOrderRouteName, new { newOrder.Id }, newOrder);
        }
        catch (ServiceBadRequestError e)
        {
            return BadRequest(new { errors = e.FieldErrors });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
    }
}
