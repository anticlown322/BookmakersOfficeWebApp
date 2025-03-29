using UserService.Application.Contracts.UseCases.Balance;
using UserService.Application.DTO.Balance;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Balance;

public class DepositToUserBalanceUseCase(IUsersRepository usersRepository) : IDepositToUserBalanceUseCase
{
    public async Task ExecuteAsync(string username, DepositRequestDto depositRequestDto, CancellationToken cancellationToken)
    {
        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        if (userToGet.Balance is null)
        {
            throw new BalanceDataIsNotFoundException(username);
        }

        userToGet.Balance.CurrentAmount += depositRequestDto.Amount;
        userToGet.Balance.LastUpdated = DateTime.UtcNow.ToUniversalTime();

        var transaction = new BalanceTransaction
        {
            UserId = userToGet.Id,
            Amount = depositRequestDto.Amount,
            CreatedAt = DateTime.UtcNow.ToUniversalTime(),
            OperationType = "Deposit",
            Comment = depositRequestDto.Comment
        };

        userToGet.Balance.Transactions.Add(transaction);
        await usersRepository.UpdateUserAsync(userToGet, cancellationToken);
    }
}