using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;
using UserService.Domain.RequestFeatures;

namespace UserService.Infrastructure.Repository.Repositories;

public class UserRepository(
    RepositoryContext repositoryContext,
    UserManager<User> userManager)
    : RepositoryBase<User>(repositoryContext), IUsersRepository
{
    public async Task<PagedList<User>> GetAllUsersAsync(
        UserParameters userParameters,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var users = await FindAllAsync(false, cancellationToken);

        var orderedUsers = users.OrderBy(p => p.UserName);

        var pagedUsers = orderedUsers
            .Skip((userParameters.PageNumber - 1) * userParameters.PageSize)
            .Take(userParameters.PageSize)
            .ToList();

        var totalCount = orderedUsers.Count();

        return new PagedList<User>(
            pagedUsers,
            totalCount,
            userParameters.PageNumber,
            userParameters.PageSize);
    }

    public async Task<User> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByIdAsync(userId.ToString());

        return user;
    }

    public async Task<User> GetUserByNameAsync(string userName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await repositoryContext.Users
            .Include(u => u.Profile)
            .Include(u => u.Balance)
            .FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);

        return user;
    }

    public async Task<IList<string>> GetUserRolesAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await userManager.GetRolesAsync(user);
    }

    public async Task<PagedList<BalanceTransaction>> GetAllBalanceTransactionsAsync(
        TransactionParameters transactionParameters, User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var userWithTransactions = await repositoryContext.Users
            .Include(u => u.Balance)
            .ThenInclude(b => b.Transactions)
            .FirstOrDefaultAsync(u => u.UserName == user.UserName, cancellationToken);

        var orderedTransactions = userWithTransactions.Balance.Transactions
            .OrderBy(p => p.CreatedAt);

        var pagedTransactions = orderedTransactions
            .Skip((transactionParameters.PageNumber - 1) * transactionParameters.PageSize)
            .Take(transactionParameters.PageSize)
            .ToList();

        var totalCount = orderedTransactions.Count();

        return new PagedList<BalanceTransaction>(
            pagedTransactions,
            totalCount,
            transactionParameters.PageNumber,
            transactionParameters.PageSize);
    }

    public async Task<IdentityResult> CreateUserAsync(User user, string password, ICollection<string> roles, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await userManager.CreateAsync(user, password);
        var registrationResult = await userManager.AddToRolesAsync(user, roles);

        return registrationResult;
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await userManager.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(User user)
    {
        await userManager.DeleteAsync(user);
    }
}