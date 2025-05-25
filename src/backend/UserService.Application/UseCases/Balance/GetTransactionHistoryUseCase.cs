using AutoMapper;
using Domain.RequestFeatures;
using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.UseCases.Balance;
using UserService.Application.DTO.Balance;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;
using UserService.Domain.RequestFeatures;

namespace UserService.Application.UseCases.Balance;

public class GetTransactionHistoryUseCase(
    IUsersRepository usersRepository,
    IMapper mapper,
    ILogger<GetTransactionHistoryUseCase> logger)
    : IGetTransactionHistory
{
    public async Task<(IEnumerable<TransactionDto> transactions, MetaData metaData)> ExecuteAsync(
        string username,
        TransactionParameters transactionParameters,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting transaction history for {username}...");

        cancellationToken.ThrowIfCancellationRequested();

        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            logger.LogWarning($"User {username} not found");

            throw new UserNotFoundByNameException(username);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var transactionsWithMetaData = await usersRepository.GetAllBalanceTransactionsAsync(
            transactionParameters,
            userToGet,
            cancellationToken);

        var transactionsDto = mapper.Map<IEnumerable<TransactionDto>>(transactionsWithMetaData);

        logger.LogInformation($"Successfully retrieved {transactionsDto.Count()} transactions for user {username}");

        return (
            transactions: transactionsDto,
            metaData: transactionsWithMetaData.MetaData);
    }
}