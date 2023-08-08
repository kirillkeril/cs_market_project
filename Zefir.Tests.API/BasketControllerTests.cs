using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Zefir.API.Contracts.Basket;
using Zefir.API.Controllers;
using Zefir.BL.Abstractions;
using Zefir.BL.Contracts.BasketDto;
using Zefir.Core.Entity;
using Zefir.Core.Errors;

namespace Zefir.Tests.API;

public class BasketControllerTests
{
    [Fact]
    public async Task GetUsersBasket_SendRequest_ShouldReturnOkAndBasket()
    {
        // Arrange
        var userId = "1";
        var mockBasketService = new Mock<IBasketService>();
        mockBasketService.Setup(s => s.GetUsersBasket(It.IsAny<int>()))
            .ReturnsAsync(new BasketInfoServiceDto(It.IsAny<int>(), new List<Product>()));
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
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task GetUsersBasket_SendRequestWithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var mockBasketService = new Mock<IBasketService>();
        var controller = new BasketController(mockBasketService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal()
                }
            }
        };

        // Act
        var result = await controller.GetUsersBasket() as UnauthorizedResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task AddProductToBasket_SendRequestWithAuth_ShouldReturnNoContent()
    {
        // Arrange
        var userId = "1";
        var productId = 123;
        var mockBasketService = new Mock<IBasketService>();
        mockBasketService.Setup(s => s.AddProductToBasket(It.IsAny<int>(), It.IsAny<int>()))
            .Returns(Task.CompletedTask);
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
        var dto = new AddProductToBasketDto(productId);

        // Act
        var result = await controller.AddProductToBasket(dto) as NoContentResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact]
    public async Task AddProductToBasket_UserNotFound_ShouldReturnNotFound()
    {
        // Arrange
        var userId = "1";
        var productId = 123;
        var mockBasketService = new Mock<IBasketService>();
        mockBasketService.Setup(s => s.AddProductToBasket(It.IsAny<int>(), It.IsAny<int>()))
            .ThrowsAsync(new ServiceNotFoundError("Product not found"));
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
        var dto = new AddProductToBasketDto(productId);

        // Act
        var result = await controller.AddProductToBasket(dto) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.NotNull(result.Value);
    }
}
