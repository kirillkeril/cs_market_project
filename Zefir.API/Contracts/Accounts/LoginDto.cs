namespace Zefir.API.Contracts.Accounts;

/// <summary>
///     Contract for login into system
/// </summary>
/// <param name="Email">Email</param>
/// <param name="Password">Password</param>
public record LoginDto(string Email, string Password);
