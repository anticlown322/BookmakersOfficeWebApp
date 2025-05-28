using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.UseCases.Balance;
using UserService.Application.DTO.Balance;
using UserService.Application.Utility;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Balance;

public class WithdrawFromUserBalanceUseCase(
    IUsersRepository usersRepository,
    ILogger<WithdrawFromUserBalanceUseCase> logger)
    : IWithdrawFromUserBalanceUseCase
{
    public async Task ExecuteAsync(
        string username,
        WithdrawRequestDto withdrawRequestDto,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Executing withdraw request for {username}...");

        cancellationToken.ThrowIfCancellationRequested();

        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            logger.LogWarning($"User {username} does not exist");

            throw new UserNotFoundByNameException(username);
        }

        if (userToGet.Balance is null)
        {
            logger.LogWarning($"Balance for user {username} is null");

            throw new BalanceDataIsNotFoundException(username);
        }

        if (withdrawRequestDto.Amount > userToGet.Balance.CurrentAmount)
        {
            logger.LogWarning("Withdraw amount is greater than current balance");

            throw new InvalidBalanceWithdrawException("Withdraw amount is greater than current balance.");
        }

        userToGet.Balance.CurrentAmount -= withdrawRequestDto.Amount;
        userToGet.Balance.LastUpdated = DateTime.UtcNow.ToUniversalTime();

        var transaction = new BalanceTransaction
        {
            UserId = userToGet.Id,
            Amount = withdrawRequestDto.Amount,
            CreatedAt = DateTime.UtcNow.ToUniversalTime(),
            OperationType = BalanceOperationTypesAndStatuses.WithdrawOperation,
            Comment = withdrawRequestDto.Comment
        };

        logger.LogInformation($"Adding balance transaction for user {username}");

        userToGet.Balance.Transactions.Add(transaction);

        cancellationToken.ThrowIfCancellationRequested();

        logger.LogInformation($"Successfully added balance transaction for user {username}");

        await usersRepository.UpdateUserAsync(userToGet, cancellationToken);
    }
}