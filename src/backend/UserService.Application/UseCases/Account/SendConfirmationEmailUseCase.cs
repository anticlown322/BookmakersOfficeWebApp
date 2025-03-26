using Microsoft.AspNetCore.Identity;
using UserService.Application.Contracts.Services;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class SendConfirmationEmailUseCase(
    IUsersRepository usersRepository,
    UserManager<Domain.Models.User> userManager,
    IEmailService emailService) : ISendConfirmationEmailUseCase
{
    public async Task ExecuteAsync(string username, string baseUrl, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var confirmationLink = $"{baseUrl}/api/users/{username}/account/confirm-email?token={token}";

        await emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);
    }
}