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
        var senderEmail = emailSettings.Value.SenderEmail;
        var senderName = emailSettings.Value.SenderName;

        await fluentEmail
            .To("karasandrey2005@gmail.com")
            .Subject("Account confirmation")
            .Body($"Please, follow this link to confirm your account: <a href='{confirmationLink}'>Confirm</a>", isHtml: true)
            .SendAsync();
    }

    public Task SendResetPasswordEmailAsync(string email, string resetPasswordLink)
    {
        throw new NotImplementedException();
    }
}