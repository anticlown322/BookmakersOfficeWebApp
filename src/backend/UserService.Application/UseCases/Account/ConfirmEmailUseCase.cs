using Microsoft.AspNetCore.Identity;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class ConfirmEmailUseCase(
    IUsersRepository usersRepository,
    UserManager<Domain.Models.User> userManager) : IConfirmEmailUseCase
{
    public async Task ExecuteAsync(string username, string token, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (result.Errors.Any())
        {
            var error = result.Errors.FirstOrDefault();
            throw new EmailCanNotBeConfirmedException(error.Description);
        }
    }
}