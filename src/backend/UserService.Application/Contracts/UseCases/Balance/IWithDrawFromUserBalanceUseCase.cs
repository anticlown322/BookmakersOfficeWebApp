using UserService.Application.DTO.Balance;

namespace UserService.Application.Contracts.UseCases.Balance;

public interface IWithDrawFromUserBalanceUseCase
{
    Task ExecuteAsync(string username, WithdrawRequestDto withdrawRequestDto, CancellationToken cancellationToken);
}