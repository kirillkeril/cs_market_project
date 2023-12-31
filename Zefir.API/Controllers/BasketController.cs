﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zefir.API.Contracts.Basket;
using Zefir.BL.Abstractions;
using Zefir.Common.Errors;

namespace Zefir.API.Controllers;

/// <summary>
///     Basket controller for user's basket
/// </summary>
[Authorize]
public class BasketController : ControllerBase
{
    private readonly IBasketService _basketService;

    /// <summary>
    /// </summary>
    /// <param name="basketService"></param>
    public BasketController(IBasketService basketService)
    {
        _basketService = basketService;
    }

    /// <summary>
    ///     Get user's bucket
    /// </summary>
    /// <returns></returns>
    [HttpGet("[controller]")]
    public async Task<IActionResult> GetUsersBasket()
    {
        var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userId is not null && int.TryParse(userId.Value, out var id))
        {
            var basket = await _basketService.GetUsersBasket(id);
            return Ok(basket);
        }

        return Unauthorized();
    }

    /// <summary>
    ///     Add product to user's basket
    /// </summary>
    /// <returns></returns>
    [HttpPost("[controller]")]
    public async Task<IActionResult> AddProductToBasket(AddProductToBasketDto dto)
    {
        try
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (int.TryParse(userId?.Value, out var id))
            {
                await _basketService.AddProductToBasket(id, dto.ProductId);
                return NoContent();
            }

            return Unauthorized();
        }
        catch (ServiceNotFoundError e)
        {
            Console.WriteLine(e);
            return NotFound(new { errors = new List<string> { e.Message } });
        }

    }

    /// <summary>
    ///     Removes product from user's basket
    /// </summary>
    /// <returns></returns>
    [HttpDelete("[controller]")]
    public async Task<IActionResult> RemoveProductFromBasket(int productId)
    {
        try
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userId is not null && int.TryParse(userId.Value, out var id))
            {
                await _basketService.RemoveProductFromBasket(id, productId);
                return NoContent();
            }

            return Unauthorized();
        }
        catch (ServiceNotFoundError e)
        {
            Console.WriteLine(e);
            return NotFound(new { errors = new List<string> { e.Message } });
        }
    }
}
