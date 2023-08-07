using Zefir.BL.Contracts.AccountDto;

namespace Zefir.BL.Abstractions;

public interface IAccountService
{
    Task<List<UserServiceDto>> GetAllUsers();
    Task<UserServiceDto> GetUserById(int id);
    Task<UserServiceDto> GetUserByEmail(string email);
    Task<AccountInfoServiceDto> Register(RegisterAccountServiceDto dto);
    Task<AccountInfoServiceDto> Login(LoginAccountServiceDto dto);
    Task<bool> DeleteAccount(LoginAccountServiceDto dto);
    Task<bool> DeleteById(int id);
    Task<AccountInfoServiceDto> RefreshToken(string accessToken, string refreshToken);
    Task<bool> Revoke(string accessToken);
}