using UserService.Application.Contracts.UseCases.Balance;
using UserService.Application.DTO.Balance;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Balance;

public class GetUserBalanceUseCase(
    IUsersRepository usersRepository)
    : IGetUserBalanceUseCase
{
    public async Task<UserBalanceGetDto> ExecuteAsync(string username, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        if (userToGet.Balance is null || userToGet.Balance?.CurrentAmount is null || userToGet.Balance?.LastUpdated is null)
        {
            throw new BalanceDataIsNotFoundException(username);
        }

        return new UserBalanceGetDto(
            userToGet.Balance.CurrentAmount,
            userToGet.Balance.LastUpdated);
    }
}