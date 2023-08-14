using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Zefir.API.Contracts.Orders;
using Zefir.API.Controllers;
using Zefir.BL.Abstractions;
using Zefir.BL.Contracts.OrdersDto;
using Zefir.Core.Entity;

namespace Zefir.Tests.API;

public class OrderControllerTests
{
    private readonly Mock<IOrderService> _mockService = new();

    [Fact]
    public async Task GetAllOrders_SendRequest_ShouldReturnOk()
    {
        // arrange
        var testUserId = It.IsAny<int>();
        _mockService.Setup(s => s.GetAllOrders(testUserId))
            .ReturnsAsync(new List<OrderInfoServiceDto>());
        var controller = new OrdersController(_mockService.Object);
        // act
        var result = await controller.GetAllOrders(testUserId) as OkObjectResult;
        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public async Task GetAllOrders_SendRequestWithoutUserId_ShouldReturnOk()
    {
        // arrange
        _mockService.Setup(s => s.GetAllOrders(null))
            .ReturnsAsync(new List<OrderInfoServiceDto>());
        var controller = new OrdersController(_mockService.Object);

        // act
        var result = await controller.GetAllOrders() as OkObjectResult;

        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public async Task GetOwnOrders_SendRequest_ShouldReturnOk()
    {
        // arrange
        var testUserId = It.IsAny<int>();
        _mockService.Setup(s => s.GetOwnOrders(testUserId))
            .ReturnsAsync(new List<OrderInfoServiceDto>());
        var controller = new OrdersController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.NameIdentifier, testUserId.ToString())
                    }))
                }
            }
        };

        // act
        var result = await controller.GetOwnOrders() as OkObjectResult;

        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public async Task GetOwnOrders_SendRequestWithoutAuth_ShouldReturnUnauthorized()
    {
        // arrange
        var testUserId = It.IsAny<int>();
        _mockService.Setup(s => s.GetOwnOrders(testUserId));
        var controller = new OrdersController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                    }))
                }
            }
        };

        // act
        var result = await controller.GetOwnOrders() as UnauthorizedResult;

        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_SendRequest_ShouldReturnCreated()
    {
        // arrange
        var testUserId = It.IsAny<int>();
        var testDto = new CreateOrderServiceDto(new[] { It.IsAny<int>() }, DateTime.Today.ToShortDateString());
        _mockService.Setup(s => s.CreateOrder(testUserId, testDto))
            .ReturnsAsync(new OrderInfoServiceDto(
                It.IsAny<int>(),
                testUserId,
                new List<Product>(),
                "Status",
                DateOnly.Parse(DateTime.Today.ToShortDateString()),
                20.0));
        var controller = new OrdersController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.NameIdentifier, testUserId.ToString())
                    }))
                }
            }
        };

        // act
        var result =
            await controller.CreateOrder(new CreateOrderDto(testDto.ProductsId, testDto.Deadline)) as
                CreatedAtRouteResult;

        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
    }

    [Fact]
    public async Task CreateOrder_SendRequestWithoutAuth_ShouldReturnUnauthorized()
    {
        // arrange
        var testUserId = It.IsAny<int>();
        var testDto = new CreateOrderServiceDto(new[] { It.IsAny<int>() }, DateTime.Today.ToShortDateString());
        _mockService.Setup(s => s.CreateOrder(testUserId, testDto))
            .ReturnsAsync(new OrderInfoServiceDto(
                It.IsAny<int>(),
                testUserId,
                new List<Product>(),
                "Status",
                DateOnly.Parse(DateTime.Today.ToShortDateString()),
                20.0));
        var controller = new OrdersController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                    }))
                }
            }
        };

        // act
        var result =
            await controller.CreateOrder(new CreateOrderDto(testDto.ProductsId, testDto.Deadline)) as
                UnauthorizedResult;

        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task UpdateOrder_SendRequest_ShouldReturnOk()
    {
        // arrange
        var testId = It.IsAny<int>();
        var testDto = new UpdateOrderServiceDto((int)Status.Done);
        _mockService.Setup(s => s.UpdateOrderStatus(testId, testDto))
            .ReturnsAsync(new OrderInfoServiceDto(testId, 1, new List<Product>(), Status.Done.ToString(),
                DateOnly.Parse(DateTime.Now.ToShortDateString()), 1));
        var controller = new OrdersController(_mockService.Object);

        // act
        var result = await controller.UpdateOrder(testId, new UpdateOrderDto(testDto.Status)) as OkObjectResult;

        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }
}
