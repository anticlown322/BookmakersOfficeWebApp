using UserService.Application.Contracts.UseCases.Balance;
using UserService.Application.DTO.Balance;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Balance;

public class WithDrawFromUserBalanceUseCase(IUsersRepository usersRepository) : IWithDrawFromUserBalanceUseCase
{
    public async Task ExecuteAsync(string username, WithdrawRequestDto withdrawRequestDto, CancellationToken cancellationToken)
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

        if (withdrawRequestDto.Amount > userToGet.Balance.CurrentAmount)
        {
            throw new InvalidBalanceWithdrawException("Withdraw amount is greater than current balance.");
        }

        userToGet.Balance.CurrentAmount -= withdrawRequestDto.Amount;
        userToGet.Balance.LastUpdated = DateTime.UtcNow.ToUniversalTime();

        var transaction = new BalanceTransaction
        {
            UserId = userToGet.Id,
            Amount = withdrawRequestDto.Amount,
            CreatedAt = DateTime.UtcNow.ToUniversalTime(),
            OperationType = "Withdraw",
            Comment = withdrawRequestDto.Comment
        };

        userToGet.Balance.Transactions.Add(transaction);
        await usersRepository.UpdateUserAsync(userToGet, cancellationToken);
    }
}