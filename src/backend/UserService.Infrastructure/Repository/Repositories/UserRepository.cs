using Microsoft.AspNetCore.Identity;
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
        var users = await FindAllAsync(false, cancellationToken);

        var orderedUsers = users.OrderBy(p => p.UserName);

        var pagedParticipants = orderedUsers
            .Skip((userParameters.PageNumber - 1) * userParameters.PageSize)
            .Take(userParameters.PageSize)
            .ToList();

        var totalCount = orderedUsers.Count();

        return new PagedList<User>(
            pagedParticipants,
            totalCount,
            userParameters.PageNumber,
            userParameters.PageSize);
    }

    public async Task<User> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        return user;
    }

    public async Task<User> GetUserByNameAsync(string userName, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByNameAsync(userName);
        return user;
    }

    public async Task<IdentityResult> CreateUserAsync(User user, string password, ICollection<string> roles, CancellationToken cancellationToken)
    {
        await userManager.CreateAsync(user, password);
        var registrationResult = await userManager.AddToRolesAsync(user, roles);
        return registrationResult;
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        await userManager.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(User user)
    {
        await userManager.DeleteAsync(user);
    }
}