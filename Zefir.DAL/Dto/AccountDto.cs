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

public record UserDataDto(string Name, string Surname, string Phone, string Email);

public record AccountResponseDto(UserDataDto? User, string? Token, string? RefreshToken, List<string>? Errors);

public record RefreshDto(string RefreshToken);
