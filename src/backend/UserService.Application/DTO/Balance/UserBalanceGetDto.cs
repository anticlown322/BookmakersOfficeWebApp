namespace UserService.Application.DTO.Balance;

public record UserBalanceGetDto(decimal CurrentBalance, DateTime LastUpdated);