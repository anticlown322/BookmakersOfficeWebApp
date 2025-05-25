using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.UseCases.Balance;
using UserService.Application.DTO.Balance;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Balance;

public class GetUserBalanceUseCase(
    IUsersRepository usersRepository,
    ILogger<GetUserBalanceUseCase> logger)
    : IGetUserBalanceUseCase
{
    public async Task<UserBalanceGetDto> ExecuteAsync(string username, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting balance data for {username}...");

        cancellationToken.ThrowIfCancellationRequested();

        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            logger.LogWarning($"User {username} not found");

            throw new UserNotFoundByNameException(username);
        }

        if (userToGet.Balance is null || userToGet.Balance?.CurrentAmount is null ||
            userToGet.Balance?.LastUpdated is null)
        {
            logger.LogWarning($"Balance data is corrupted for user {username}");

            throw new BalanceDataIsNotFoundException(username);
        }

        logger.LogInformation($"Successfully retrieved balance data for user {username}");

        return new UserBalanceGetDto(
            userToGet.Balance.CurrentAmount,
            userToGet.Balance.LastUpdated);
    }
}