using UserService.Application.DTO.Balance;

namespace UserService.Application.Contracts.UseCases.Balance;

public interface IGetUserBalanceUseCase
{
    Task<UserBalanceGetDto> ExecuteAsync(string username, CancellationToken cancellationToken);
}