namespace Zefir.API.Contracts.Accounts;

/// <summary>
///     Contract for create an account
/// </summary>
/// <param name="Name">User's name</param>
/// <param name="Surname">User's surname</param>
/// <param name="Phone">Phone</param>
/// <param name="Email">Email (must be unique)</param>
/// <param name="Password">Password (more than 8 chars)</param>
/// <param name="PasswordConfirm">Must be same with password</param>
public record CreateAccountDto(
    string Name,
    string Surname,
    string Phone,
    string Email,
    string Password,
    string PasswordConfirm
);
