using System.Net;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.Validation.Exceptions.Base;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class ConfirmEmailUseCase(
    IUsersRepository usersRepository,
    UserManager<Domain.Models.User> userManager) : IConfirmEmailUseCase
{
    public async Task ExecuteAsync(string username, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        if (user.EmailConfirmed)
        {
            throw new EmailCanNotBeConfirmedException($"Your email is already confirmed.");
        }

        cancellationToken.ThrowIfCancellationRequested();

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

        cancellationToken.ThrowIfCancellationRequested();

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (result.Errors.Any())
        {
            var error = result.Errors.FirstOrDefault();
            throw new EmailCanNotBeConfirmedException(error.Description);
        }
    }
}