using UserService.Application.DTO.Balance;
using UserService.Application.DTO.User;
using UserService.Domain.Models;
using UserService.Domain.RequestFeatures;

namespace UserService.Tests.UnitTests.UseCases.Balance;

public static class BalanceUseCasesTestData
{
    public static Guid ValidUserId = Guid.NewGuid();

    public static Domain.Models.User ValidUser => new()
    {
        Id = ValidUserId.ToString(),
        UserName = "testUser",
        Balance = new UserBalance { CurrentAmount = 100.50m }
    };

    public static UserGetDto ValidUserDto => new()
    {
        UserName = "testUser"
    };

    public static PagedList<BalanceTransaction> CreateTestTransactions(int count)
    {
        var transactions = Enumerable.Range(1, count)
            .Select(i => new BalanceTransaction
            {
                Id = i,
                Amount = i * 10m,
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            })
            .ToList();

        return new PagedList<BalanceTransaction>(
            transactions,
            count,
            1,
            count);
    }

    public static IEnumerable<TransactionDto> CreateTestTransactionDtos(int count) =>
        Enumerable.Range(1, count)
            .Select(i => new TransactionDto
            {
                Amount = i * 10,
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            });

    public static Domain.Models.User CreateUserWithBalance(decimal initialAmount)
    {
        return new Domain.Models.User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Balance = new UserBalance
            {
                CurrentAmount = initialAmount,
                LastUpdated = DateTime.UtcNow,
                Transactions = new List<BalanceTransaction>()
            }
        };
    }

    public static Domain.Models.User CreateUserWithoutBalance()
    {
        return new Domain.Models.User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Balance = null
        };
    }
}