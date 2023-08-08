using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Zefir.API.Contracts.Accounts;
using Zefir.API.Controllers;
using Zefir.BL.Abstractions;
using Zefir.BL.Contracts.AccountDto;
using Zefir.Core.Entity;
using Zefir.Core.Errors;

namespace Zefir.Tests.API;

public class AccountControllerTests
{
    private readonly Mock<IAccountService> _mockService = new();

    [Fact]
    public async Task GetAll_SendResultByAdmin_ShouldReturnListOfUsers()
    {
        //arrange
        var mockAccountService = new Mock<IAccountService>();
        mockAccountService.Setup(s => s.GetAllUsers())
            .ReturnsAsync(new List<UserServiceDto>
            {
                new(1, "Test", "Tester", "+19999999999", "test@mail.com", 0, Role.UserRole),
                new(2, "Test2nd", "Tester2", "+29999999999", "test2@mail.com", 0.1, Role.AdminRole)
            });
        var controller = new AccountsController(mockAccountService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.Name, "admin"),
                        new(ClaimTypes.NameIdentifier, "3"),
                        new(ClaimTypes.Role, Role.AdminRole)
                    }))
                }
            }
        };
        //act
        var result = await controller.GetAll() as OkObjectResult;
        //assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task GetAll_SendRequestByUser_ShouldReturnUnauthorized()
    {
        //arrange
        _mockService.Setup(s => s.GetAllUsers())
            .ReturnsAsync(new List<UserServiceDto>
            {
                new(1, "Test", "Tester", "+19999999999", "test@mail.com", 0, Role.UserRole),
                new(2, "Test2nd", "Tester2", "+29999999999", "test2@mail.com", 0.1, Role.AdminRole)
            });

        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.Name, "user"),
                        new(ClaimTypes.NameIdentifier, "1"),
                        new(ClaimTypes.Role, Role.UserRole)
                    }))
                }
            }
        };

        //act
        var result = await controller.GetAll() as StatusCodeResult;
        //assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task GetById_RequestByAdmin_ShouldReturnUser()
    {
        //arrange
        _mockService.Setup(s => s.GetUserById(It.IsAny<int>()))
            .ReturnsAsync(new UserServiceDto(1, "Test", "Tester", "+19999999999", "test@test.com", 0, Role.UserRole));

        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.NameIdentifier, "1"),
                        new(ClaimTypes.Name, "Admin"),
                        new(ClaimTypes.Role, Role.AdminRole)
                    }))
                }
            }
        };

        //act
        var result = await controller.GetById(1) as OkObjectResult;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task GetById_RequestByUser_ShouldReturnForbidden()
    {
        //arrange
        _mockService.Setup(s => s.GetUserById(It.IsAny<int>()))
            .ReturnsAsync(new UserServiceDto(1, "Test", "Tester", "+19999999999", "test@test.com", 0, Role.UserRole));

        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.NameIdentifier, "2"),
                        new(ClaimTypes.Name, "user"),
                        new(ClaimTypes.Role, Role.UserRole)
                    }))
                }
            }
        };

        //act
        var result = await controller.GetById(1) as StatusCodeResult;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task GetById_UserNotFound_ShouldReturnNotFound()
    {
        //Arrange
        _mockService.Setup(s => s.GetUserById(It.IsAny<int>()))
            .ThrowsAsync(new ServiceNotFoundError("User not found"));

        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.NameIdentifier, "1"),
                        new(ClaimTypes.Name, "Admin"),
                        new(ClaimTypes.Role, Role.AdminRole)
                    }))
                }
            }
        };

        //act
        var result = await controller.GetById(It.IsAny<int>()) as NotFoundObjectResult;

        //assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task GetUserByEmail_SendRequestByAdmin_ShouldReturnUser()
    {
        //arrange
        _mockService.Setup(s => s.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(new UserServiceDto(1, "Test", "Tester", "+19999999999", "test@test.com", 0, Role.UserRole));

        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.NameIdentifier, "1"),
                        new(ClaimTypes.Name, "Admin"),
                        new(ClaimTypes.Role, Role.AdminRole)
                    }))
                }
            }
        };

        //act
        var result = await controller.GetByEmail(new GetUserByEmailDto(It.IsAny<string>())) as OkObjectResult;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task GetByEmail_RequestByUser_ShouldReturnForbidden()
    {
        //arrange
        _mockService.Setup(s => s.GetUserByEmail(It.IsAny<string>()))
            .ReturnsAsync(new UserServiceDto(1, "Test", "Tester", "+19999999999", "test@test.com", 0, Role.UserRole));

        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.NameIdentifier, "2"),
                        new(ClaimTypes.Name, "user"),
                        new(ClaimTypes.Role, Role.UserRole)
                    }))
                }
            }
        };

        //act
        var result = await controller.GetByEmail(new GetUserByEmailDto(It.IsAny<string>())) as StatusCodeResult;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task GetUserByEmail_UserNotFound_ShouldReturnNotFound()
    {
        //arrange
        _mockService.Setup(s => s.GetUserByEmail(It.IsAny<string>()))
            .ThrowsAsync(new ServiceNotFoundError("User is not found"));

        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.NameIdentifier, "1"),
                        new(ClaimTypes.Name, "Admin"),
                        new(ClaimTypes.Role, Role.AdminRole)
                    }))
                }
            }
        };

        //act
        var result = await controller.GetByEmail(new GetUserByEmailDto(It.IsAny<string>())) as NotFoundObjectResult;

        //Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task Register_SendRequest_ShouldReturnUser()
    {
        _mockService.Setup(s => s
                .Register(new RegisterAccountServiceDto(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())))
            .ReturnsAsync(new AccountInfoServiceDto(
                new UserServiceDto(
                    It.IsAny<int>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<double>(),
                    Role.UserRole),
                It.IsAny<string>(),
                It.IsAny<string>(),
                null
            ));

        var controller = new AccountsController(_mockService.Object);

        //act
        var result = await controller.Register(new CreateAccountDto(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>())) as OkObjectResult;

        //assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task Register_ProvidedInvalidData_ShouldReturnBadRequest()
    {
        _mockService
            .Setup(s => s
                .Register(new RegisterAccountServiceDto(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())))
            .ThrowsAsync(new ServiceBadRequestError(("asd", "ads")));

        var controller = new AccountsController(_mockService.Object);

        //act
        var result = await controller.Register(new CreateAccountDto(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>())) as BadRequestObjectResult;

        //assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task Register_ProvidedInvalidDataInList_ShouldReturnBadRequest()
    {
        _mockService
            .Setup(s => s
                .Register(new RegisterAccountServiceDto(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>())))
            .ReturnsAsync(new AccountInfoServiceDto(null, null, null, new List<string> { "Error" }));

        var controller = new AccountsController(_mockService.Object);

        //act
        var result = await controller.Register(new CreateAccountDto(
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<string>())) as BadRequestObjectResult;

        //assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task Login_ProvidedValidData_ShouldReturnUserWithTokens()
    {
        var testUser = new LoginAccountServiceDto("test@test.com", "password");
        _mockService.Setup(s => s.Login(testUser))
            .ReturnsAsync(
                new AccountInfoServiceDto(
                    new UserServiceDto(1, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                        It.IsAny<string>(), It.IsAny<double>(), Role.UserRole),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    null
                )
            );
        var controller = new AccountsController(_mockService.Object);

        var result = await controller.Login(new LoginDto(testUser.Email, testUser.Password)) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task Login_ProvidedInvalidData_ShouldReturnBadRequest()
    {
        var testUser = new LoginAccountServiceDto("test@test.com", "password");
        _mockService.Setup(s => s.Login(testUser))
            .ThrowsAsync(new ServiceBadRequestError(("Field", "error")));
        var controller = new AccountsController(_mockService.Object);

        var result = await controller.Login(new LoginDto(testUser.Email, testUser.Password)) as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task Login_ProvidedInvalidDataInList_ShouldReturnBadRequest()
    {
        var testUser = new LoginAccountServiceDto("test@test.com", "password");
        _mockService.Setup(s => s.Login(testUser))
            .ReturnsAsync(new AccountInfoServiceDto(null, null, null, new List<string> { "Error" }));
        var controller = new AccountsController(_mockService.Object);

        var result = await controller.Login(new LoginDto(testUser.Email, testUser.Password)) as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task DeleteAccount_ProvidedValidData_ShouldReturnUserWithTokens()
    {
        var testUser = new LoginAccountServiceDto("test@test.com", "password");
        _mockService.Setup(s => s.DeleteAccount(testUser))
            .ReturnsAsync(true);
        var controller = new AccountsController(_mockService.Object);

        var result =
            await controller.DeleteAccount(new DeleteAccountDto(testUser.Email, testUser.Password)) as NoContentResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact]
    public async Task DeleteAccount_ProvidedInvalidData_ShouldReturnUserWithTokens()
    {
        var testUser = new LoginAccountServiceDto("test@test.com", "password");
        _mockService.Setup(s => s.DeleteAccount(testUser))
            .ReturnsAsync(false);
        var controller = new AccountsController(_mockService.Object);

        var result =
            await controller.DeleteAccount(new DeleteAccountDto(testUser.Email, testUser.Password)) as BadRequestResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task DeleteById_ProvidedValidDataWithAdmin_ShouldReturnUserWithTokens()
    {
        var testUser = new LoginAccountServiceDto("test@test.com", "password");
        _mockService.Setup(s => s.DeleteById(It.IsAny<int>()))
            .ReturnsAsync(true);
        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.Role, Role.AdminRole),
                        new(ClaimTypes.Name, testUser.Email),
                        new(ClaimTypes.NameIdentifier, "1")
                    }))
                }
            }
        };

        var result = await controller.DeleteById(It.IsAny<int>()) as NoContentResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact]
    public async Task DeleteById_ProvidedInvalidDataWithAdmin_ShouldReturnUserWithTokens()
    {
        var testUser = new LoginAccountServiceDto("test@test.com", "password");
        _mockService.Setup(s => s.DeleteById(It.IsAny<int>()))
            .ReturnsAsync(false);
        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.Role, Role.AdminRole),
                        new(ClaimTypes.Name, testUser.Email),
                        new(ClaimTypes.NameIdentifier, "1")
                    }))
                }
            }
        };

        var result = await controller.DeleteById(It.IsAny<int>()) as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public async Task DeleteById_ProvidedDataWithoutAdmin_ShouldReturnUserWithTokens()
    {
        var testUser = new LoginAccountServiceDto("test@test.com", "password");
        _mockService.Setup(s => s.DeleteById(It.IsAny<int>()));
        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new(ClaimTypes.Role, Role.UserRole),
                        new(ClaimTypes.Name, testUser.Email),
                        new(ClaimTypes.NameIdentifier, "1")
                    }))
                }
            }
        };

        var result = await controller.DeleteById(It.IsAny<int>()) as StatusCodeResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status403Forbidden, result.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_ProvideValidOldToken_ShouldReturnOkAndNewToken()
    {
        _mockService.Setup(s => s.RefreshToken(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new AccountInfoServiceDto(
                new UserServiceDto(It.IsAny<int>(), "", "", "", "", 1, Role.UserRole),
                null, null, null));
        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request = { Headers = { Authorization = "Bearer SomeOldToken" } }
                }
            }
        };

        var result = await controller.RefreshToken(new RefreshAccessTokenDto("refresh")) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_ProvideInvalidOldToken_ShouldReturnUnauthorized()
    {
        _mockService.Setup(s => s.RefreshToken("AnyToken", "AnyRefreshToken"));
        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request = { Headers = { Authorization = "" } }
                }
            }
        };

        var result = await controller.RefreshToken(new RefreshAccessTokenDto("refresh")) as UnauthorizedObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_ProvideInvalidRefreshToken_ShouldReturnUnauthorized()
    {
        _mockService.Setup(s => s.RefreshToken(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new AccountInfoServiceDto(null, null, null, new List<string> { "error" }));
        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request = { Headers = { Authorization = "Bearer someToken" } }
                }
            }
        };

        var result =
            await controller.RefreshToken(new RefreshAccessTokenDto("invalid refresh")) as UnauthorizedObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task Logout_ValidRequest_ShouldReturnNoContent()
    {
        _mockService.Setup(s => s.Revoke(It.IsAny<string>()))
            .ReturnsAsync(true);
        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request = { Headers = { Authorization = "Bearer someToken" } }
                }
            }
        };

        var result = await controller.Logout() as NoContentResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact]
    public async Task Logout_InvalidRequest_ShouldReturnBadRequest()
    {
        _mockService.Setup(s => s.Revoke(It.IsAny<string>()))
            .ReturnsAsync(false);
        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request = { Headers = { Authorization = "Bearer someToken" } }
                }
            }
        };

        var result = await controller.Logout() as BadRequestResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public async Task Logout_InvalidToken_ShouldReturnUnauthorized()
    {
        _mockService.Setup(s => s.Revoke(It.IsAny<string>()));
        var controller = new AccountsController(_mockService.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    Request = { Headers = { Authorization = "" } }
                }
            }
        };

        var result = await controller.Logout() as UnauthorizedObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status401Unauthorized, result.StatusCode);
    }
}
