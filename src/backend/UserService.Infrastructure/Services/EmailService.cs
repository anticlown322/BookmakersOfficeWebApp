using FluentEmail.Core;
using Microsoft.Extensions.Options;
using UserService.Application.Contracts.Services;
using UserService.Domain.Models;

namespace UserService.Infrastructure.Services;

public class EmailService(
    IOptions<EmailSettings> emailSettings,
    IFluentEmail fluentEmail)
    : IEmailService
{
    public async Task SendConfirmationEmailAsync(string email, string confirmationLink)
    {
        await fluentEmail
            .To(email)
            .Subject("Account confirmation")
            .Body($"Please, follow this link to confirm your account: <a href='{confirmationLink}'>Confirm</a>", isHtml: true)
            .SendAsync();
    }

    public async Task SendResetPasswordEmailAsync(string email, string resetCode)
    {
        await fluentEmail
            .To(email)
            .Subject("Password reset code")
            .Body($"Password reset code: {resetCode}")
            .SendAsync();
    }
}