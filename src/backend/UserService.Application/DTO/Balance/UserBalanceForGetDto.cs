namespace UserService.Application.DTO.Balance;

public record UserBalanceForGetDto(decimal CurrentBalance, DateTime LastUpdated);