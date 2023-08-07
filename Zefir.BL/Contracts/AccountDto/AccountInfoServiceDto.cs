namespace Zefir.BL.Contracts.AccountDto;

public record AccountInfoServiceDto(
    UserServiceDto? User,
    string? Token,
    string? RefreshToken,
    List<string>? Errors);
