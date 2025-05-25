using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.UseCases.Authentication;
using UserService.Application.DTO.Authentication;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Authentication;

public class LogoutUseCase(
    IUsersRepository usersRepository,
    ILogger<LogoutUseCase> logger)
    : ILogoutUseCase
{
    public async Task ExecuteAsync(UserLogoutDto userLogoutDto, bool populateExp, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Logging out for user {userLogoutDto.UserName}...");

        cancellationToken.ThrowIfCancellationRequested();

        var user = await usersRepository.GetUserByNameAsync(userLogoutDto.UserName, cancellationToken);
        if (user is null)
        {
            logger.LogWarning($"User with username {userLogoutDto.UserName} was not found");

            throw new UserNotFoundByNameException(userLogoutDto.UserName);
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.Now.ToUniversalTime();

        cancellationToken.ThrowIfCancellationRequested();

        await usersRepository.UpdateUserAsync(user, cancellationToken);

        logger.LogInformation($"User with username {user.UserName} successfully logged out");
    }
}