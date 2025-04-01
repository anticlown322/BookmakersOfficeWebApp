using UserService.Application.DTO.Balance;

namespace UserService.Application.Contracts.UseCases.Balance;

public interface IWithdrawFromUserBalanceUseCase
{
    Task ExecuteAsync(string username, WithdrawRequestDto withdrawRequestDto, CancellationToken cancellationToken);
}