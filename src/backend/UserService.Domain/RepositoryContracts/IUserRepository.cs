using Microsoft.AspNetCore.Identity;
using UserService.Domain.Models;
using UserService.Domain.RequestFeatures;

namespace UserService.Domain.RepositoryContracts;

public interface IUsersRepository
{
    Task<PagedList<User>> GetAllUsersAsync(UserParameters userParameters, CancellationToken cancellationToken);
    Task<User> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<User> GetUserByNameAsync(string userName, CancellationToken cancellationToken);
    Task<IList<string>> GetUserRolesAsync(User user, CancellationToken cancellationToken);
    Task<PagedList<BalanceTransaction>> GetAllBalanceTransactionsAsync(
        TransactionParameters transactionParameters, User user, CancellationToken cancellationToken);
    Task<IdentityResult> CreateUserAsync(User user, string password, ICollection<string> roles, CancellationToken cancellationToken);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken);
    Task DeleteUserAsync(User user);
}