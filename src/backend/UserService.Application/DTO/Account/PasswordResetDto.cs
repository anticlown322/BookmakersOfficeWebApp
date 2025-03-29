namespace UserService.Application.DTO.Account;

public record PasswordResetDto(string token, string newPassword);