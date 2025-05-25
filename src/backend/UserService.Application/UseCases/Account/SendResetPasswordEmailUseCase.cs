using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.Services;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class SendResetPasswordEmailUseCase(
    IUsersRepository usersRepository,
    IEmailService emailService,
    ILogger<SendResetPasswordEmailUseCase> logger)
    : ISendResetPasswordEmailUseCase
{
    public async Task ExecuteAsync(string username, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Attempting to send reset password email for {username}");

        cancellationToken.ThrowIfCancellationRequested();

        var user = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (user is null)
        {
            logger.LogWarning($"User with username {username} was not found");

            throw new UserNotFoundByNameException(username);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var resetToken = await usersRepository.GeneratePasswordResetTokenAsync(user, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        await emailService.SendResetPasswordEmailAsync(user.Email, resetToken);

        logger.LogInformation($"Successfully sent reset password email for {username}");
    }
}