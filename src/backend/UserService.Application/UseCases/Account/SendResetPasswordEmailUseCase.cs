using Microsoft.AspNetCore.Identity;
using UserService.Application.Contracts.Services;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class SendResetPasswordEmailUseCase(
    IUsersRepository usersRepository,
    UserManager<Domain.Models.User> userManager,
    IEmailService emailService) : ISendResetPasswordEmailUseCase
{
    public async Task ExecuteAsync(string username, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

        await emailService.SendResetPasswordEmailAsync(user.Email, resetToken);
    }
}