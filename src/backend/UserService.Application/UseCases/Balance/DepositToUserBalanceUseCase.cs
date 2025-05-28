using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.UseCases.Balance;
using UserService.Application.DTO.Balance;
using UserService.Application.Utility;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Balance;

public class DepositToUserBalanceUseCase(
    IUsersRepository usersRepository,
    ILogger<DepositToUserBalanceUseCase> logger)
    : IDepositToUserBalanceUseCase
{
    public async Task ExecuteAsync(
        string username,
        DepositRequestDto depositRequestDto,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Depositing to balance of {username}...");

        cancellationToken.ThrowIfCancellationRequested();

        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            logger.LogWarning($"User {username} not found");

            throw new UserNotFoundByNameException(username);
        }

        if (userToGet.Balance is null)
        {
            logger.LogWarning($"Balance doesn't exist for {username}");

            throw new BalanceDataIsNotFoundException(username);
        }

        userToGet.Balance.CurrentAmount += depositRequestDto.Amount;
        userToGet.Balance.LastUpdated = DateTime.UtcNow.ToUniversalTime();

        var transaction = new BalanceTransaction
        {
            UserId = userToGet.Id,
            Amount = depositRequestDto.Amount,
            CreatedAt = DateTime.UtcNow.ToUniversalTime(),
            OperationType = BalanceOperationTypesAndStatuses.DepositOperation,
            Comment = depositRequestDto.Comment
        };

        logger.LogInformation($"Making transaction for balance of {username}...");

        userToGet.Balance.Transactions.Add(transaction);

        cancellationToken.ThrowIfCancellationRequested();

        await usersRepository.UpdateUserAsync(userToGet, cancellationToken);

        logger.LogInformation($"Successfully updated balance for use {username}");
    }
}