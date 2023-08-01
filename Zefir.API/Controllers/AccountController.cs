using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zefir.DAL.Dto;
using Zefir.DAL.Errors;
using Zefir.DAL.Services;
using Zefir.Core.Entity;

namespace Zefir.API.Controllers;

/// <summary>
/// Accounts api controller
/// </summary>
[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private const string RegisterRouteName = "register";
    private const string LoginRouteName = "login";
    private const string LogoutRouteName = "logout";
    private const string RefreshTokenRouteName = "refresh-token";
    private const string DeleteAccountRouteName = "delete-account";
    private const string DeleteAccountByIdRouteName = "delete-account-by-id";


    private readonly AccountService _accountService;

    /// <summary>
    /// </summary>
    /// <param email="accountService"></param>
    /// <param name="accountService"></param>
    public AccountController(AccountService accountService)
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
        var users = await _accountService.GetAllUsers();
        if (users.Count == 0) return NoContent();
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
            var user = await _accountService.GetUserById(id);
            return Ok(user);
        }
        catch (ServiceNotFoundError e)
        {
            return NotFound(new { errors = new List<string> { e.Message } });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
    }

    /// <summary>
    /// Get user by email (admin only)
    /// </summary>
    /// <param name="email">string email</param>
    /// <returns>Ok with user OR 404 with errors OR 500 with errors</returns>
    [Authorize(Roles = Role.AdminRole)]
    [HttpGet("{email}")]
    public async Task<IActionResult> GetByEmail(string email)
    {
        try
        {
            var user = await _accountService.GetUserByEmail(email);
            return Ok(user);
        }
        catch (ServiceNotFoundError e)
        {
            return NotFound(new { errors = new List<string> { e.Message } });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
    }

    /// <summary>
    ///     Gets data and add new user
    /// </summary>
    /// <param name="dto">User fields <see cref="RegisterDto" /></param>
    /// <returns>User and jwt token OR list of errors <see cref="PublicAccountDataDto" /></returns>
    [HttpPost("register", Name = RegisterRouteName)]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        try
        {
            var result = await _accountService.Register(dto);
            if (result.Errors is not null) return BadRequest(new { result.Errors });
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
    }

    /// <summary>
    ///     Login with email and password
    /// </summary>
    /// <param name="dto">Login data <see cref="AccountDto" /></param>
    /// <returns>User and token OR list of errors <see cref="AccountDto" /></returns>
    [HttpPost("login", Name = LoginRouteName)]
    [AllowAnonymous]
    public async Task<IActionResult> Login(AccountDto dto)
    {
        try
        {
            var result = await _accountService.Login(dto);
            if (result.Errors is not null) return Unauthorized(new { result.Errors });
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
    }

    /// <summary>
    ///     Removes user's account
    /// </summary>
    /// <param name="dto">Login data</param>
    /// <returns>Bad request if users not found OR no content if user deleted</returns>
    [HttpDelete("delete", Name = DeleteAccountRouteName)]
    [Authorize]
    public async Task<IActionResult> DeleteAccount(AccountDto dto)
    {
        try
        {
            var result = await _accountService.DeleteAccount(dto);
            if (result == false) return BadRequest();
            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
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
        try
        {
            var result = await _accountService.DeleteById(id);
            if (result) return NoContent();
            return NotFound(new { });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
    }

    /// <summary>
    ///     Return new pair of access and refresh tokens;
    /// </summary>
    /// <param name="refreshToken">User's refresh token</param>
    /// <returns>Unauthorized with list of errors OR login data <see cref="AccountDto" /></returns>
    [AllowAnonymous]
    [HttpPost("refresh", Name = RefreshTokenRouteName)]
    public async Task<IActionResult> RefreshToken(RefreshDto refreshToken)
    {
        try
        {
            var authHeader = HttpContext.Request.Headers.Authorization;
            if (string.IsNullOrWhiteSpace(authHeader))
                return Unauthorized(new { errors = new List<string> { "Authorization header is undefined" } });
            var accessToken = authHeader.ToString().Split(" ")[1];
            var result = await _accountService.RefreshToken(accessToken, refreshToken.RefreshToken);
            if (result.Errors != null) return Unauthorized(new { result.Errors });
            return Ok(result);
        }
        catch (Exception e)
        {
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
    }

    /// <summary>
    ///     Logout and delete refresh token
    /// </summary>
    /// <returns>NoContent or BadRequest</returns>
    [HttpPatch("logout", Name = LogoutRouteName)]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var authHeader = HttpContext.Request.Headers.Authorization;
            if (string.IsNullOrWhiteSpace(authHeader))
                return Unauthorized(new { errors = new List<string> { "Authorization header is undefined" } });
            var accessToken = authHeader.ToString().Split(" ")[1];
            var result = await _accountService.Revoke(accessToken);
            if (result == false) return BadRequest();
            return NoContent();
        }
        catch (Exception e)
        {
            return StatusCode(500, new { errors = new List<string> { e.Message } });
        }
    }
}
