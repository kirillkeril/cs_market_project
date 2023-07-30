using Zefir.Domain.Entity;

namespace Zefir.DAL.Dto;

public record AccountDto(string Email, string Password);

public record LoginResponseDto(User? User, string? Token, string? RefreshToken, List<string>? Errors);

public record RegisterDto(
    string Name,
    string Surname,
    string Phone,
    string Email,
    string Password,
    string PasswordConfirm
);

public record RegisterResponseDto(User? User, string? Token, string? RefreshToken, List<string>? Errors);

public record RefreshDto(string RefreshToken);
