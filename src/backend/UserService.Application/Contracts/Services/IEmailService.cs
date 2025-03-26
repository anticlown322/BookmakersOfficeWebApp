namespace UserService.Application.Contracts.Services;

public interface IEmailService
{
    Task SendConfirmationEmailAsync(string email, string confirmationLink);
    Task SendResetPasswordEmailAsync(string email, string resetPasswordLink);
}