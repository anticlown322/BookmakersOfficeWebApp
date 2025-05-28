using FluentEmail.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.Application.Contracts.Services;
using UserService.Domain.Models;

namespace UserService.Infrastructure.Services;

public class EmailService(
    IOptions<EmailSettings> emailSettings,
    IFluentEmail fluentEmail,
    ILogger<EmailService> logger)
    : IEmailService
{
    public async Task SendConfirmationEmailAsync(string email, string confirmationLink)
    {
        logger.LogInformation($"Send confirmation of email {email}...");

        await fluentEmail
            .To(email)
            .Subject("Account confirmation")
            .Body(
                $"Please, follow this link to confirm your account: <a href='{confirmationLink}'>Confirm</a>",
                isHtml: true)
            .SendAsync();

        logger.LogInformation($"Successfully sent confirmation of email {email}...");
    }

    public async Task SendResetPasswordEmailAsync(string email, string resetCode)
    {
        logger.LogInformation($"Send reset password of email {email}...");

        await fluentEmail
            .To(email)
            .Subject("Password reset code")
            .Body($"Password reset code: {resetCode}")
            .SendAsync();

        logger.LogInformation($"Successfully sent password reset code {resetCode}...");
    }
}