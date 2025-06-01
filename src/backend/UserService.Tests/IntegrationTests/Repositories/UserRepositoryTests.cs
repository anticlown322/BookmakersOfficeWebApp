using UserService.Domain.Models;
using UserService.Domain.RequestFeatures;
using UserService.Infrastructure.Repository.Repositories;
using UserService.Infrastructure.Utility;
using UserService.Tests.IntegrationTests.Fixtures;

namespace UserService.Tests.IntegrationTests.Repositories;

public class UserRepositoryTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly UserRepository _userRepository;

    public UserRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _userRepository = new UserRepository(
            _fixture.DbContext,
            _fixture.UserManager);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldCreateUser()
    {
        // Arrange
        var user = new User
        {
            UserName = "test_user",
            Email = "test@example.com",
            RefreshToken = "token123",
            RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
        };

        var password = "Password123!";
        var roles = new[] { UserRoles.Gambler };

        // Act
        var result = await _userRepository.CreateUserAsync(user, password, roles);

        // Assert
        result.Succeeded.Should().BeTrue();

        var dbUser = await _fixture.UserManager.FindByNameAsync(user.UserName);
        dbUser.Should().NotBeNull();
        dbUser!.Email.Should().Be(user.Email);

        var userRoles = await _fixture.UserManager.GetRolesAsync(dbUser);
        userRoles.Should().Contain(UserRoles.Gambler);
    }

    [Fact]
    public async Task GetUserByNameAsync_ShouldReturnUserWithProfileAndBalance()
    {
        // Arrange
        var user = new User
        {
            UserName = "test_user2",
            Email = "test2@example.com",
            Profile = new UserProfile { FirstName = "John", LastName = "Doe" },
            Balance = new UserBalance { CurrentAmount = 100 }
        };

        await _fixture.UserManager.CreateAsync(user);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var foundUser = await _userRepository.GetUserByNameAsync(user.UserName);

        // Assert
        foundUser.Should().NotBeNull();
        foundUser!.Profile.Should().NotBeNull();
        foundUser.Profile.FirstName.Should().Be("John");
        foundUser.Profile.LastName.Should().Be("Doe");
        foundUser.Balance.CurrentAmount.Should().Be(100);
    }

    [Fact]
    public async Task GetAllBalanceTransactionsAsync_ShouldReturnPagedList()
    {
        // Arrange
        var user = new User
        {
            UserName = "transaction_user",
            Email = "transaction_user@example.com",
            Balance = new UserBalance
            {
                CurrentAmount = 100,
                LastUpdated = DateTime.UtcNow,
                Transactions = new List<BalanceTransaction>
                {
                    new BalanceTransaction
                    {
                        Amount = 10,
                        CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                        OperationType = "Deposit"
                    },
                    new BalanceTransaction
                    {
                        Amount = 20,
                        CreatedAt = DateTime.UtcNow.AddMinutes(-20),
                        OperationType = "Deposit"
                    },
                    new BalanceTransaction
                    {
                        Amount = 30,
                        CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                        OperationType = "Withdrawal"
                    }
                }
            }
        };

        var createResult = await _fixture.UserManager.CreateAsync(user);
        createResult.Succeeded.Should().BeTrue();
        await _fixture.DbContext.SaveChangesAsync();

        var parameters = new TransactionParameters { PageNumber = 1, PageSize = 2 };

        // Act
        var pagedList = await _userRepository.GetAllBalanceTransactionsAsync(parameters, user);

        // Assert
        pagedList.Should().NotBeNull();
        pagedList.Count.Should().Be(2);

        pagedList[0].Amount.Should().Be(10);
        pagedList[1].Amount.Should().Be(20);

        // Verify pagination
        var secondPage = await _userRepository.GetAllBalanceTransactionsAsync(
            new TransactionParameters { PageNumber = 2, PageSize = 2 },
            user);

        secondPage.Count.Should().Be(1);
        secondPage[0].Amount.Should().Be(30);
    }
}