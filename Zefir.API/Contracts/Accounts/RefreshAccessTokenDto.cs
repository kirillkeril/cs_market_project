namespace Zefir.API.Contracts.Accounts;

/// <summary>
///     Contract for refresh user's access token
/// </summary>
/// <param name="RefreshToken">User's refresh token</param>
public record RefreshAccessTokenDto(string RefreshToken);
