namespace Zefir.BL.Contracts.AccountDto;

public record RegisterAccountServiceDto(
    string Name,
    string Surname,
    string Phone,
    string Email,
    string Password,
    string PasswordConfirm
);
