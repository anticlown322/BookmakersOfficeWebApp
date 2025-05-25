using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.DTO.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class ResetPasswordUseCase(
    IUsersRepository usersRepository,
    ILogger<ResetPasswordUseCase> logger)
    : IResetPasswordUseCase
{
    public async Task ExecuteAsync(
        string username,
        PasswordResetDto passwordResetDto,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Resetting password for {username}");

        cancellationToken.ThrowIfCancellationRequested();

        var user = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (user is null)
        {
            logger.LogWarning($"User not found by username {username}");

            throw new UserNotFoundByNameException(username);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var result = await usersRepository.ResetPasswordAsync(
            user,
            passwordResetDto.Token,
            passwordResetDto.NewPassword,
            cancellationToken);
        if (result.Errors.Any())
        {
            logger.LogWarning($"Errors while resetting password: {result.Errors}");

            var error = result.Errors.FirstOrDefault();
            throw new PasswordCanNotBeReset(error.Description);
        }

        logger.LogInformation($"Successfully reset password for {username}");
    }
}