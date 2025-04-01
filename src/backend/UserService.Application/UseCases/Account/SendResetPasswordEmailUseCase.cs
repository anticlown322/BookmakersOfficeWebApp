using UserService.Application.Contracts.Services;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class SendResetPasswordEmailUseCase(
    IUsersRepository usersRepository,
    IEmailService emailService) : ISendResetPasswordEmailUseCase
{
    public async Task ExecuteAsync(string username, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var resetToken = await usersRepository.GeneratePasswordResetTokenAsync(user, cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        await emailService.SendResetPasswordEmailAsync(user.Email, resetToken);
    }
}