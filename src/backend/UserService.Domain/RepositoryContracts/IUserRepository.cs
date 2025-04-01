using Microsoft.AspNetCore.Identity;
using UserService.Domain.Models;
using UserService.Domain.RequestFeatures;

namespace UserService.Domain.RepositoryContracts;

public interface IUsersRepository
{
    Task<PagedList<User>> GetAllUsersAsync(UserParameters userParameters, CancellationToken cancellationToken = default);
    Task<User> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User> GetUserByNameAsync(string userName, CancellationToken cancellationToken = default);
    Task<IList<string>> GetUserRolesAsync(User user, CancellationToken cancellationToken = default);
    Task<PagedList<BalanceTransaction>> GetAllBalanceTransactionsAsync(
        TransactionParameters transactionParameters, User user, CancellationToken cancellationToken = default);
    Task<IdentityResult> CreateUserAsync(User user, string password, ICollection<string> roles, CancellationToken cancellationToken = default);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(User user);
    Task<string> GenerateEmailConfirmationTokenAsync(User user, CancellationToken cancellationToken = default);
    Task<IdentityResult> ConfirmEmailAsync(User user, string token, CancellationToken cancellationToken = default);
    Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword, CancellationToken cancellationToken = default);
    Task<string> GeneratePasswordResetTokenAsync(User user, CancellationToken cancellationToken = default);
}