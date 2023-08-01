namespace Zefir.DAL.Dto;

public record AccountDto(string Email, string Password);

public record RegisterDto(
    string Name,
    string Surname,
    string Phone,
    string Email,
    string Password,
    string PasswordConfirm
);

public record PublicUserData(int Id, string Name, string Surname, string Phone, string Email, string Role);

public record PublicAccountDataDto(
    PublicUserData? User,
    string? Token,
    string? RefreshToken,
    List<string>? Errors);

public record RefreshDto(string RefreshToken);
