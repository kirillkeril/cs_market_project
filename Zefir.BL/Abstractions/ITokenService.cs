using System.Security.Claims;
using Zefir.Core.Entity;

namespace Zefir.BL.Abstractions;

public interface ITokenService
{
    public string BuildToken(User user);
    public string BuildRefreshToken();
    ClaimsPrincipal GetClaimsFromExpiredToken(string token);
}
