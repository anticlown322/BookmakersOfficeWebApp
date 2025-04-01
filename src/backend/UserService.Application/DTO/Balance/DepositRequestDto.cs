namespace UserService.Application.DTO.Balance;

public record DepositRequestDto(decimal Amount, string? Comment = null);