using AutoMapper;
using UserService.Application.Contracts.UseCases.Balance;
using UserService.Application.DTO.Balance;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;
using UserService.Domain.RequestFeatures;

namespace UserService.Application.UseCases.Balance;

public class GetTransactionHistoryUseCase(
    IUsersRepository usersRepository,
    IMapper mapper
    ) : IGetTransactionHistory
{
    public async Task<(IEnumerable<TransactionDto> transactions, MetaData metaData)> ExecuteAsync(
        string username, TransactionParameters transactionParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var transactionsWithMetaData = await usersRepository.GetAllBalanceTransactionsAsync(
            transactionParameters, userToGet, cancellationToken);

        var transactionsDto = mapper.Map<IEnumerable<TransactionDto>>(transactionsWithMetaData);

        return (
            transactions: transactionsDto,
            metaData: transactionsWithMetaData.MetaData);
    }
}