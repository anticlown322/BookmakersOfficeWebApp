using UserService.Application.DTO.Balance;

namespace UserService.Application.Contracts.UseCases.Balance;

public interface IGetTransactionHistory
{
    Task<TransactionHistoryForGetDto> ExecuteAsync(string username, CancellationToken cancellationToken);
}