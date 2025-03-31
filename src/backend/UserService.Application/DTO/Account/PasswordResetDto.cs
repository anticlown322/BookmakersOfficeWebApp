namespace UserService.Application.DTO.Account;

public record PasswordResetDto(string Token, string NewPassword);