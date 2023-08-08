using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zefir.API.Contracts.Accounts;
using Zefir.BL.Abstractions;
using Zefir.BL.Contracts.AccountDto;
using Zefir.Core.Entity;
using Zefir.Core.Errors;

namespace Zefir.API.Controllers;

/// <summary>
/// Accounts api controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class AccountsController : ControllerBase
{
    private const string RegisterRouteName = "register";
    private const string LoginRouteName = "login";
    private const string LogoutRouteName = "logout";
    private const string RefreshTokenRouteName = "refresh-token";
    private const string DeleteAccountRouteName = "delete-account";
    private const string DeleteAccountByIdRouteName = "delete-account-by-id";


    private readonly IAccountService _accountService;

    /// <summary>
    /// </summary>
    /// <param email="accountService"></param>
    /// <param name="accountService"></param>
    public AccountsController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// Get all users (admin only)
    /// </summary>
    /// <returns>204 OR 200 with users</returns>
    [Authorize(Roles = Role.AdminRole)]
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var currentUserRole = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        if (currentUserRole is null || !currentUserRole.Value.Equals(Role.AdminRole))
            return StatusCode(StatusCodes.Status403Forbidden);
        var users = await _accountService.GetAllUsers();
        return Ok(users);
    }

    /// <summary>
    /// Get user by id (admin only)
    /// </summary>
    /// <param name="id">integer id</param>
    /// <returns>200 with user OR 404 with errors OR 500 with errors</returns>
    [Authorize(Roles = Role.AdminRole)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var currentUserRole = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Role));
            if (currentUserRole is null || !currentUserRole.Value.Equals(Role.AdminRole))
                return StatusCode(StatusCodes.Status403Forbidden);
            var user = await _accountService.GetUserById(id);
            return Ok(user);
        }
        catch (ServiceNotFoundError e)
        {
            Console.WriteLine(e);
            return NotFound(new { errors = new List<string> { e.Message } });
        }
    }

    /// <summary>
    /// Get user by email (admin only)
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Ok with user OR 404 with errors OR 500 with errors</returns>
    [Authorize(Roles = Role.AdminRole)]
    [HttpGet("{email}")]
    public async Task<IActionResult> GetByEmail(GetUserByEmailDto dto)
    {
        try
        {
            var currentUserRole = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Role));
            if (currentUserRole is null || !currentUserRole.Value.Equals(Role.AdminRole))
                return StatusCode(StatusCodes.Status403Forbidden);

            var user = await _accountService.GetUserByEmail(dto.Email);
            return Ok(user);
        }
        catch (ServiceNotFoundError e)
        {
            Console.WriteLine(e);
            return NotFound(new { errors = new List<string> { e.Message } });
        }
    }

    /// <summary>
    ///     Gets data and add new user
    /// </summary>
    /// <param name="dto">User fields <see cref="RegisterAccountServiceDto" /></param>
    /// <returns>User and jwt token OR list of errors <see cref="AccountInfoServiceDto" /></returns>
    [HttpPost("register", Name = RegisterRouteName)]
    [AllowAnonymous]
    public async Task<IActionResult> Register(CreateAccountDto dto)
    {
        try
        {
            var serviceContract = new RegisterAccountServiceDto(
                dto.Name,
                dto.Surname,
                dto.Phone,
                dto.Email,
                dto.Password,
                dto.PasswordConfirm);
            var result = await _accountService.Register(serviceContract);
            if (result.Errors is not null) return BadRequest(new { result.Errors });
            return Ok(result);
        }
        catch (ServiceBadRequestError e)
        {
            Console.WriteLine(e);
            return BadRequest(new { errors = e.FieldErrors });
        }
    }

    /// <summary>
    ///     Login with email and password
    /// </summary>
    /// <param name="dto">Login data <see cref="LoginAccountServiceDto" /></param>
    /// <returns>User and token OR list of errors <see cref="LoginAccountServiceDto" /></returns>
    [HttpPost("login", Name = LoginRouteName)]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        try
        {
            var serviceContract = new LoginAccountServiceDto(dto.Email, dto.Password);
            var result = await _accountService.Login(serviceContract);
            if (result.Errors is not null) return BadRequest(new { result.Errors });
            return Ok(result);
        }
        catch (ServiceBadRequestError e)
        {
            Console.WriteLine(e);
            return BadRequest(new
            {
                errors = e.FieldErrors
            });
        }
    }

    /// <summary>
    ///     Removes user's account
    /// </summary>
    /// <param name="dto">Login data</param>
    /// <returns>Bad request if users not found OR no content if user deleted</returns>
    [HttpDelete("delete", Name = DeleteAccountRouteName)]
    [Authorize]
    public async Task<IActionResult> DeleteAccount(DeleteAccountDto dto)
    {
        var serviceContract = new LoginAccountServiceDto(dto.Email, dto.Password);
        var result = await _accountService.DeleteAccount(serviceContract);
        if (result == false) return BadRequest();
        return NoContent();
    }

    /// <summary>
    /// Delete user by id (admin only)
    /// </summary>
    /// <param name="id">integer id</param>
    /// <returns>204 OR 404 OR 500 with errors</returns>
    [Authorize(Roles = Role.AdminRole)]
    [HttpDelete("delete/{id:int}", Name = DeleteAccountByIdRouteName)]
    public async Task<IActionResult> DeleteById(int id)
    {
        var currentUser = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Role));
        if (currentUser is null || !currentUser.Value.Equals(Role.AdminRole))
            return StatusCode(StatusCodes.Status403Forbidden);

        var result = await _accountService.DeleteById(id);
        if (result) return NoContent();
        return NotFound(new { });
    }

    /// <summary>
    ///     Return new pair of access and refresh tokens;
    /// </summary>
    /// <param name="refreshToken">User's refresh token</param>
    /// <returns>Unauthorized with list of errors OR login data <see cref="LoginAccountServiceDto" /></returns>
    [AllowAnonymous]
    [HttpPost("refresh", Name = RefreshTokenRouteName)]
    public async Task<IActionResult> RefreshToken(RefreshAccessTokenDto refreshToken)
    {
        var authHeader = HttpContext.Request.Headers.Authorization;
        if (string.IsNullOrWhiteSpace(authHeader))
            return Unauthorized(new { errors = new List<string> { "Authorization header is undefined" } });
        var accessToken = authHeader.ToString().Split(" ")[1];
        var result = await _accountService.RefreshToken(accessToken, refreshToken.RefreshToken);
        if (result.Errors != null) return Unauthorized(new { result.Errors });
        return Ok(result);
    }

    /// <summary>
    ///     Logout and delete refresh token
    /// </summary>
    /// <returns>NoContent or BadRequest</returns>
    [HttpPatch("logout", Name = LogoutRouteName)]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var authHeader = HttpContext.Request.Headers.Authorization;
        if (string.IsNullOrWhiteSpace(authHeader))
            return Unauthorized(new { errors = new List<string> { "Authorization header is undefined" } });
        var accessToken = authHeader.ToString().Split(" ")[1];
        var result = await _accountService.Revoke(accessToken);
        if (result == false) return BadRequest();
        return NoContent();
    }
}
