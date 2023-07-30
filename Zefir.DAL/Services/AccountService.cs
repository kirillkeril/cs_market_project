using Microsoft.EntityFrameworkCore;
using Zefir.DAL.Dto;
using Zefir.DAL.Errors;
using Zefir.DAL.Interfaces;
using Zefir.Domain.Entity;

namespace Zefir.DAL.Services;

public class AccountService
{
    private readonly AppDbContext _appContext;
    private readonly ITokenService _tokenService;

    public AccountService(AppDbContext appContext, ITokenService tokenService)
    {
        _appContext = appContext;
        _tokenService = tokenService;
    }

    public async Task<List<User>> GetAllUsers()
    {
        var users = await _appContext.Users.ToListAsync();
        return users;
    }

    public async Task<User> GetUserById(int id)
    {
        var user = await _appContext.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) throw new ServiceNotFoundError("No user with such id");
        return user;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var user = await _appContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user is null) throw new ServiceNotFoundError("No user with such email");
        return user;
    }

    /// <summary>
    ///     Get user data for new user
    /// </summary>
    /// <param name="dto">Data for registration <see cref="RegisterDto" /></param>
    /// <returns>Null if user exists or <see cref="RegisterResponseDto" /></returns>
    public async Task<RegisterResponseDto> Register(RegisterDto dto)
    {
        var errors = new List<string>();
        if (!string.IsNullOrWhiteSpace(dto.Password) && !dto.Password.Equals(dto.PasswordConfirm))
            errors.Add("Password is not confirmed");

        var candidate = await _appContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (candidate is not null) errors.Add("User already exists");

        if (errors.Count > 0) return new RegisterResponseDto(null, null, null, errors);

        var refreshToken = _tokenService.BuildRefreshToken();
        var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password, 10);
        candidate = new User(dto.Name, dto.Surname, dto.Phone, dto.Email, hashedPassword)
            { RefreshToken = refreshToken };
        var token = _tokenService.BuildToken(candidate);

        await _appContext.Users.AddAsync(candidate);
        await _appContext.SaveChangesAsync();
        return new RegisterResponseDto(candidate, token, refreshToken, null);
    }

    public async Task<LoginResponseDto> Login(AccountDto dto)
    {
        var errors = new List<string>();

        var candidate =
            await _appContext.Users.FirstOrDefaultAsync(u =>
                u.Email == dto.Email);
        if (
            candidate != null &&
            BCrypt.Net.BCrypt.EnhancedVerify(dto.Password, candidate.HashedPassword))
        {
            var token = _tokenService.BuildToken(candidate);
            var refreshToken = _tokenService.BuildRefreshToken();
            candidate.RefreshToken = refreshToken;
            await _appContext.SaveChangesAsync();
            return new LoginResponseDto(candidate, token, refreshToken, null);
        }

        errors.Add("Invalid email or password");
        return new LoginResponseDto(null, null, null, errors);
    }

    public async Task<bool> DeleteAccount(AccountDto dto)
    {
        var candidate = await _appContext.Users.FirstOrDefaultAsync(
            u =>
                u.Email.Equals(dto.Email));
        if (candidate is null) return false;
        var isSame = BCrypt.Net.BCrypt.EnhancedVerify(dto.Password, candidate.HashedPassword);
        if (!isSame) return false;

        _appContext.Users.Remove(candidate);
        await _appContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteById(int id)
    {
        var candidate = await _appContext.Users.FirstOrDefaultAsync(u => u.Id.Equals(id));
        if (candidate is not null) return true;
        return false;
    }

    public async Task<LoginResponseDto> RefreshToken(string accessToken, string refreshToken)
    {
        var errors = new List<string>();
        var principal = _tokenService.GetClaimsFromExpiredToken(accessToken);
        var username = principal.Identity?.Name;
        if (username is null) errors.Add("Unauthorized");

        var user = await _appContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(username));
        if (user is null || user.RefreshToken != refreshToken)
        {
            errors.Add("Invalid token");
            return new LoginResponseDto(null, null, null, errors);
        }

        var newAccessToken = _tokenService.BuildToken(user);
        var newRefreshToken = _tokenService.BuildRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _appContext.SaveChangesAsync();

        return new LoginResponseDto(user, newAccessToken, newAccessToken, null);
    }

    public async Task<bool> Revoke(string accessToken)
    {
        var claims = _tokenService.GetClaimsFromExpiredToken(accessToken);
        var username = claims.Identity?.Name;
        if (username is null) return false;

        var user = await _appContext.Users.FirstOrDefaultAsync(u => u.Email.Equals(username));

        if (user is null) return false;

        user.RefreshToken = null;
        await _appContext.SaveChangesAsync();
        return true;
    }
}
