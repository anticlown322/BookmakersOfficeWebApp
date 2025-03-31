namespace UserService.Application.DTO.Balance;

public record WithdrawRequestDto(decimal Amount, string ConfirmationCode, string? Comment = null);