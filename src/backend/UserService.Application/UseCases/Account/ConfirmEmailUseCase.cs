using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class ConfirmEmailUseCase(
    IUsersRepository usersRepository,
    ILogger<ConfirmEmailUseCase> logger)
    : IConfirmEmailUseCase
{
    public async Task ExecuteAsync(string username, CancellationToken cancellationToken)
    {
        logger.LogInformation("Attempting to confirm email...");

        cancellationToken.ThrowIfCancellationRequested();

        var user = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (user is null)
        {
            logger.LogWarning($"User not found by username {username}");

            throw new UserNotFoundByNameException(username);
        }

        if (user.EmailConfirmed)
        {
            logger.LogWarning($"User ({username}) email is already confirmed");

            throw new EmailCanNotBeConfirmedException("Your email is already confirmed.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var token = await usersRepository.GenerateEmailConfirmationTokenAsync(user, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        var result = await usersRepository.ConfirmEmailAsync(user, token, cancellationToken);
        if (result.Errors.Any())
        {
            logger.LogWarning($"Errors while confirming email: {result.Errors}");

            var error = result.Errors.FirstOrDefault();
            throw new EmailCanNotBeConfirmedException(error.Description);
        }

        logger.LogInformation("Successfully confirmed email");
    }
}