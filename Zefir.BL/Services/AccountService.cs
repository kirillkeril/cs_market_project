using Microsoft.EntityFrameworkCore;
using Zefir.BL.Abstractions;
using Zefir.BL.Contracts.AccountDto;
using Zefir.Core.Entity;
using Zefir.Core.Errors;
using Zefir.DAL;

namespace Zefir.BL.Services;

public class AccountService : IAccountService
{
    private readonly AppDbContext _appContext;
    private readonly ITokenService _tokenService;

    public AccountService(AppDbContext appContext, ITokenService tokenService)
    {
        _appContext = appContext;
        _tokenService = tokenService;
    }

    public async Task<List<UserServiceDto>> GetAllUsers()
    {
        var users = await _appContext.Users.Include(user => user.Role).ToListAsync();
        var publicUsers = new List<UserServiceDto>();
        foreach (var user in users)
            publicUsers.Add(new UserServiceDto(user.Id, user.Name, user.Surname, user.Phone, user.Email, user.Sale,
                user.Role.Name));
        return publicUsers;
    }

    public async Task<UserServiceDto> GetUserById(int id)
    {
        var user = await _appContext.Users.Include(user => user.Role).FirstOrDefaultAsync(u => u.Id == id);
        if (user is null) throw new ServiceNotFoundError("No user with such id");
        var publicUser =
            new UserServiceDto(user.Id, user.Name, user.Surname, user.Phone, user.Email, user.Sale, user.Role.Name);
        return publicUser;
    }

    public async Task<UserServiceDto> GetUserByEmail(string email)
    {
        var user = await _appContext.Users.Include(user => user.Role).FirstOrDefaultAsync(u => u.Email == email);
        if (user is null) throw new ServiceNotFoundError("No user with such email");
        var publicUser =
            new UserServiceDto(user.Id, user.Name, user.Surname, user.Phone, user.Email, user.Sale, user.Role.Name);
        return publicUser;
    }

    /// <summary>
    ///     Get user data for new user
    /// </summary>
    /// <param name="dto">Data for registration <see cref="RegisterAccountServiceDto" /></param>
    /// <returns>Null if user exists or <see cref="AccountInfoServiceDto" /></returns>
    public async Task<AccountInfoServiceDto> Register(RegisterAccountServiceDto dto)
    {
        var errors = new List<string>();
        if (!string.IsNullOrWhiteSpace(dto.Password) && !dto.Password.Equals(dto.PasswordConfirm))
            errors.Add("Password is not confirmed");

        var candidate = await _appContext.Users.Include(user => user.Role)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (candidate is not null) errors.Add("User already exists");

        if (errors.Count > 0) return new AccountInfoServiceDto(null, null, null, errors);

        var refreshToken = _tokenService.BuildRefreshToken();

        var userRole = _appContext.Roles.FirstOrDefault(r => r.Name.Equals(Role.UserRole));
        if (userRole is null)
        {
            userRole = new Role(Role.UserRole);
            await _appContext.Roles.AddAsync(userRole);
        }

        var adminRole = _appContext.Roles.FirstOrDefault(r => r.Name.Equals(Role.AdminRole));
        if (adminRole is null)
        {
            adminRole = new Role(Role.AdminRole);
            await _appContext.Roles.AddAsync(adminRole);
        }

        var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password, 10);
        candidate = new User(dto.Name, dto.Surname, dto.Phone, dto.Email, hashedPassword)
            { RefreshToken = refreshToken, Role = adminRole };
        await _appContext.Users.AddAsync(candidate);
        await _appContext.SaveChangesAsync();

        var token = _tokenService.BuildToken(candidate);
        var userData = new UserServiceDto(
            candidate.Id,
            candidate.Name,
            candidate.Surname,
            candidate.Phone,
            candidate.Email,
            candidate.Sale,
            candidate.Role.Name
        );
        await _appContext.Baskets.AddAsync(new Basket { User = candidate });
        await _appContext.SaveChangesAsync();
        return new AccountInfoServiceDto(userData, token, refreshToken, null);
    }

    public async Task<AccountInfoServiceDto> Login(LoginAccountServiceDto dto)
    {
        var errors = new List<string>();

        var candidate =
            await _appContext.Users.Include(u => u.Role).FirstOrDefaultAsync(u =>
                u.Email == dto.Email);
        if (
            candidate != null &&
            BCrypt.Net.BCrypt.EnhancedVerify(dto.Password, candidate.HashedPassword))
        {
            var token = _tokenService.BuildToken(candidate);
            var refreshToken = _tokenService.BuildRefreshToken();
            candidate.RefreshToken = refreshToken;
            await _appContext.SaveChangesAsync();
            var userData = new UserServiceDto(
                candidate.Id,
                candidate.Name,
                candidate.Surname,
                candidate.Phone,
                candidate.Email,
                candidate.Sale,
                candidate.Role.Name
            );
            return new AccountInfoServiceDto(userData, token, refreshToken, null);
        }

        errors.Add("Invalid email or password");
        return new AccountInfoServiceDto(null, null, null, errors);
    }

    public async Task<bool> DeleteAccount(LoginAccountServiceDto dto)
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

    public async Task<AccountInfoServiceDto> RefreshToken(string accessToken, string refreshToken)
    {
        var errors = new List<string>();
        var principal = _tokenService.GetClaimsFromExpiredToken(accessToken);
        var username = principal.Identity?.Name;
        if (username is null) errors.Add("Unauthorized");

        var user = await _appContext.Users.Include(user => user.Role)
            .FirstOrDefaultAsync(u => u.Email.Equals(username));
        if (user is null || user.RefreshToken != refreshToken)
        {
            errors.Add("Invalid token");
            return new AccountInfoServiceDto(null, null, null, errors);
        }

        var newAccessToken = _tokenService.BuildToken(user);
        var newRefreshToken = _tokenService.BuildRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _appContext.SaveChangesAsync();

        var userData = new UserServiceDto(user.Id, user.Name, user.Surname, user.Phone, user.Email, user.Sale,
            user.Role.Name);
        return new AccountInfoServiceDto(userData, newAccessToken, newRefreshToken, null);
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
