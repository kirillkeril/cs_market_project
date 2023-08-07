using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Zefir.API.Contracts.Basket;
using Zefir.API.Controllers;
using Zefir.BL.Abstractions;
using Zefir.BL.Contracts;
using Zefir.Core.Entity;
using Zefir.Core.Errors;

namespace Zefir.Tests;

public class BasketControllerTests
{
    [Fact]
    public async Task GetUsersBasket_Should_Return_OK_With_Basket()
    {
        // Arrange
        var userId = "1"; // Пример идентификатора пользователя
        var mockBasketService = new Mock<IBasketService>();
        mockBasketService.Setup(s => s.GetUsersBasket(It.IsAny<int>()))
            .ReturnsAsync(new PublicBasketData(It.IsAny<int>(), new List<Product>()));
        var controller = new BasketController(mockBasketService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.NameIdentifier, userId)
                    }))
                }
            }
        };

        // Act
        var result = await controller.GetUsersBasket() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Value); // Проверяем, что корзина не равна null или пустая
    }

    [Fact]
    public async Task GetUsersBasket_Should_Return_Unauthorized_When_User_Not_Authenticated()
    {
        // Arrange
        var mockBasketService = new Mock<IBasketService>();
        var controller = new BasketController(mockBasketService.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal()
            }
        };

        // Act
        var result = await controller.GetUsersBasket() as UnauthorizedResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task AddProductToBasket_Should_Return_NoContent_On_Successful_Addition()
    {
        // Arrange
        var userId = "1"; // Пример идентификатора пользователя
        var productId = 123; // Пример идентификатора товара
        var mockBasketService = new Mock<IBasketService>();
        mockBasketService.Setup(s => s.AddProductToBasket(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);
        var controller = new BasketController(mockBasketService.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.NameIdentifier, userId)
                }))
            }
        };
        var dto = new AddProductToBasketDto(productId);

        // Act
        var result = await controller.AddProductToBasket(dto) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact]
    public async Task AddProductToBasket_Should_Return_NotFound_When_ServiceNotFoundError()
    {
        // Arrange
        var userId = "1"; // Пример идентификатора пользователя
        var productId = 123; // Пример идентификатора товара
        var mockBasketService = new Mock<IBasketService>();
        mockBasketService.Setup(s => s.AddProductToBasket(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new ServiceNotFoundError("Товар не найден"));
        var controller = new BasketController(mockBasketService.Object);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new(ClaimTypes.NameIdentifier, userId)
                }))
            }
        };
        var dto = new AddProductToBasketDto(productId);

        // Act
        var result = await controller.AddProductToBasket(dto) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.NotNull(result.Value);
    }
}
