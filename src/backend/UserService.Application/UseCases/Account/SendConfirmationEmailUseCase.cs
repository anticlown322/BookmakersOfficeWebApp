using Microsoft.AspNetCore.Identity;
using UserService.Application.Contracts.Services;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class SendConfirmationEmailUseCase(
    IUsersRepository usersRepository,
    IEmailService emailService) : ISendConfirmationEmailUseCase
{
    public async Task ExecuteAsync(string username, string baseUrl, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        if (user.EmailConfirmed)
        {
            throw new EmailCanNotBeConfirmedException($"Your email is already confirmed.");
        }

        var confirmationLink = $"{baseUrl}/api/users/{username}/account/confirm-email";

        await emailService.SendConfirmationEmailAsync(user.Email, confirmationLink);
    }
}