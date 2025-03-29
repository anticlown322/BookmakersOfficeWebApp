namespace UserService.Application.DTO.Balance;

public record WithdrawRequestDto(decimal Amount, string? Comment = null, string ConfirmationCode = null);