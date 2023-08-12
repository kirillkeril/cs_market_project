using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Zefir.API.Contracts.Categories;
using Zefir.API.Controllers;
using Zefir.BL.Abstractions;
using Zefir.BL.Contracts.CategoryDto;
using Zefir.Common.Errors;
using Zefir.Core.Entity;

namespace Zefir.Tests.API;

public class CategoriesControllerTests
{
    private readonly Mock<ICategoryService> _mockService = new();

    [Fact]
    public async Task GetAll_ShouldReturnOk()
    {
        // arrange
        _mockService.Setup(s => s.GetAllCategories())
            .ReturnsAsync(new List<Category>());
        var controller = new CategoriesController(_mockService.Object);
        // act
        var result = await controller.GetAll() as OkObjectResult;
        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public async Task GetByName_ShouldReturnOk()
    {
        // arrange
        _mockService.Setup(s => s.GetCategoryByName(It.IsAny<string>()))
            .ReturnsAsync(new Category(It.IsAny<string>(), It.IsAny<string>()));
        var controller = new CategoriesController(_mockService.Object);
        // act
        var result = await controller.GetByName(It.IsAny<string>()) as OkObjectResult;
        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public async Task Create_WhenAdmin_ShouldReturnOk()
    {
        // arrange
        _mockService.Setup(s => s.CreateNewCategory(
                new CreateCategoryServiceDto("Some", "Some")))
            .ReturnsAsync(new Category("Some", "Some"));
        var controller = new CategoriesController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(new Claim[]
                        {
                            new(ClaimTypes.NameIdentifier, "1"),
                            new(ClaimTypes.Role, Role.AdminRole)
                        })
                    )
                }
            }
        };
        // act
        var result =
            await controller.Create(new CreateCategoryServiceDto("Some", "Some")) as
                CreatedAtRouteResult;
        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
    }

    [Fact]
    public async Task Create_WhenExists_ShouldReturnBadRequest()
    {
        // arrange
        _mockService.Setup(s => s.CreateNewCategory(
                new CreateCategoryServiceDto("Some", "Some")))
            .ThrowsAsync(new ServiceBadRequestError(("Name", "Already exists")));
        var controller = new CategoriesController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(new Claim[]
                        {
                            new(ClaimTypes.NameIdentifier, "1"),
                            new(ClaimTypes.Role, Role.AdminRole)
                        })
                    )
                }
            }
        };
        // act
        var result =
            await controller.Create(new CreateCategoryServiceDto("Some", "Some")) as
                BadRequestObjectResult;
        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Create_WhenNotAdmin_ShouldReturnForbidden()
    {
        // arrange
        _mockService.Setup(s => s.CreateNewCategory(
                new CreateCategoryServiceDto("Some", "Some")))
            .ThrowsAsync(new ServiceBadRequestError(("Name", "Already exists")));
        var controller = new CategoriesController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(new Claim[]
                        {
                            new(ClaimTypes.NameIdentifier, "1"),
                            new(ClaimTypes.Role, Role.UserRole)
                        })
                    )
                }
            }
        };
        // act
        var result =
            await controller.Create(new CreateCategoryServiceDto("Some", "Some")) as
                StatusCodeResult;
        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task Update_WhenAdmin_ShouldReturnOk()
    {
        // arrange
        _mockService.Setup(s => s.UpdateCategory("some", new UpdateCategoryServiceDto("Some new", "some new")))
            .ReturnsAsync(new Category("Some new", "some new"));
        var controller = new CategoriesController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.Role, Role.AdminRole)
                    }))
                }
            }
        };

        // act
        var result = await controller.Update("Some", new UpdateCategoryDto("Some new", "Some new")) as OkObjectResult;

        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public async Task Update_WhenNotFound_ShouldReturnServiceNotFoundError()
    {
        // arrange
        var testName = "testName";
        var testNewName = "testNewName";
        var testNewDescription = "testNewDescription";
        var apiContract = new UpdateCategoryDto(testNewName, testNewDescription);
        var serviceContract = new UpdateCategoryServiceDto(apiContract.Name, apiContract.Description);

        _mockService.Setup(s => s.UpdateCategory(testName, serviceContract))
            .ThrowsAsync(new ServiceNotFoundError("some error"));
        var controller = new CategoriesController(_mockService.Object);

        //act
        var act = async () => await controller.Update(testName, apiContract);

        // assert
        await Assert.ThrowsAsync<ServiceNotFoundError>(act);
    }

    [Fact]
    public async Task Update_WhenBadRequest_ShouldReturnServiceBadRequestError()
    {
        // arrange
        var testName = "testName";
        var testNewName = "testNewName";
        var testNewDescription = "testNewDescription";
        var apiContract = new UpdateCategoryDto(testNewName, testNewDescription);
        var serviceContract = new UpdateCategoryServiceDto(apiContract.Name, apiContract.Description);

        _mockService.Setup(s => s.UpdateCategory(testName, serviceContract))
            .ThrowsAsync(new ServiceBadRequestError(("field", "some error")));
        var controller = new CategoriesController(_mockService.Object);

        //act
        var act = async () => await controller.Update(testName, apiContract);

        // assert
        await Assert.ThrowsAsync<ServiceBadRequestError>(act);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent()
    {
        // arrange
        var testName = "test";
        _mockService.Setup(s => s.DeleteCategory(testName))
            .ReturnsAsync(true);
        var controller = new CategoriesController(_mockService.Object);

        // act
        var result = await controller.Delete(testName) as NoContentResult;

        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Delete_WhenNotFound_ShouldReturnNotFound()
    {
        // arrange
        var testName = "test";
        _mockService.Setup(s => s.DeleteCategory(testName))
            .ReturnsAsync(false);
        var controller = new CategoriesController(_mockService.Object);

        // act
        var result = await controller.Delete(testName) as NotFoundObjectResult;

        // assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }
}
