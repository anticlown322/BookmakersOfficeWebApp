using UserService.Application.DTO.Balance;

namespace UserService.Application.Contracts.UseCases.Balance;

public interface IDepositToUserBalanceUseCase
{
    Task ExecuteAsync(string username, DepositRequestDto depositRequestDto, CancellationToken cancellationToken);
}