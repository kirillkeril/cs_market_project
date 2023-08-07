namespace Zefir.BL.Contracts.AccountDto;

public record UserServiceDto(
    int Id,
    string Name,
    string Surname,
    string Phone,
    string Email,
    double Sale,
    string Role
);
