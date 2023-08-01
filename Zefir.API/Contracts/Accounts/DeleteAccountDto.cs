namespace Zefir.API.Contracts.Accounts;

/// <summary>
///     Contract for delete user's account (email and password confirmation)
/// </summary>
/// <param name="Email">User's email</param>
/// <param name="Password">User's password</param>
public record DeleteAccountDto(string Email, string Password);
