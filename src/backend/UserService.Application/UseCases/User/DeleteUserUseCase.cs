using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.UseCases.User;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.User;

public class DeleteUserUseCase(
    IUsersRepository usersRepository,
    ILogger<DeleteUserUseCase> logger)
    : IDeleteUserUseCase
{
    public async Task ExecuteAsync(string userName, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Deleting user {userName}...");

        cancellationToken.ThrowIfCancellationRequested();

        var userToDelete = await usersRepository.GetUserByNameAsync(userName, cancellationToken);
        if (userToDelete is null)
        {
            logger.LogWarning($"User {userName} was not found");

            throw new UserNotFoundByNameException(userName);
        }

        cancellationToken.ThrowIfCancellationRequested();

        await usersRepository.DeleteUserAsync(userToDelete);

        logger.LogInformation($"Successfully deleted user {userName}");
    }
}