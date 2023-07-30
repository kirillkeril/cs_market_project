using System.Security.Claims;
using Zefir.Domain.Entity;

namespace Zefir.DAL.Interfaces;

public interface ITokenService
{
    public string BuildToken(User user);
    public string BuildRefreshToken();
    ClaimsPrincipal GetClaimsFromExpiredToken(string token);
}
