using AutoMapper;
using UserService.Application.Contracts.UseCases.Balance;
using UserService.Application.DTO.Balance;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Balance;

public class GetTransactionHistoryUseCase(
    IUsersRepository usersRepository,
    IMapper mapper
    ) : IGetTransactionHistory
{
    public async Task<TransactionHistoryForGetDto> ExecuteAsync(string username, CancellationToken cancellationToken)
    {
        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        if (userToGet.Balance?.Transactions is null || !userToGet.Balance.Transactions.Any())
        {
            return new TransactionHistoryForGetDto(
                Transactions: Array.Empty<TransactionDto>(),
                TotalCount: 0);
        }

        var transactions = userToGet.Balance.Transactions
            .OrderByDescending(t => t.CreatedAt)
            .ToList();

        var mappedTransactions = mapper.Map<List<TransactionDto>>(transactions);

        return new TransactionHistoryForGetDto(
            Transactions: mappedTransactions.AsReadOnly(),
            TotalCount: mappedTransactions.Count);
    }
}