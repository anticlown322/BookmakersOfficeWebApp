using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.Application.Contracts.Services;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class SendConfirmationEmailUseCase(
    IUsersRepository usersRepository,
    IEmailService emailService,
    ILogger<SendConfirmationEmailUseCase> logger,
    IOptions<EmailSettings> emailSettings)
    : ISendConfirmationEmailUseCase
{
    public async Task ExecuteAsync(string username, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Sending confirmation email for {username}");

        cancellationToken.ThrowIfCancellationRequested();

        var user = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (user is null)
        {
            logger.LogWarning($"User with username {username} not found");

            throw new UserNotFoundByNameException(username);
        }

        if (user.EmailConfirmed)
        {
            logger.LogInformation($"Email is already confirmed for {username}");

            throw new EmailCanNotBeConfirmedException($"Your email is already confirmed.");
        }

        var confirmationLink = $"{emailSettings.Value.ConfirmEmailBaseUrl}/email-confirmation/{username}";

        cancellationToken.ThrowIfCancellationRequested();

        await emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);

        logger.LogInformation($"Successfully sent confirmation email for {username}");
    }
}