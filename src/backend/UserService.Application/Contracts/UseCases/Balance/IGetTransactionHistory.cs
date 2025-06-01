using UserService.Application.DTO.Balance;
using UserService.Domain.RequestFeatures;

namespace UserService.Application.Contracts.UseCases.Balance;

public interface IGetTransactionHistory
{
    Task<(IEnumerable<TransactionDto> transactions, MetaData metaData)> ExecuteAsync(
        string username, TransactionParameters transactionParameters, CancellationToken cancellationToken);
}