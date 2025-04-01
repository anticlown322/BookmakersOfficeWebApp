using Microsoft.AspNetCore.Identity;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.DTO.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class ResetPasswordUseCase(
    IUsersRepository usersRepository)
    : IResetPasswordUseCase
{
    public async Task ExecuteAsync(string username, PasswordResetDto passwordResetDto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var result = await usersRepository.ResetPasswordAsync(user, passwordResetDto.Token, passwordResetDto.NewPassword, cancellationToken);
        if (result.Errors.Any())
        {
            var error = result.Errors.FirstOrDefault();
            throw new PasswordCanNotBeReset(error.Description);
        }
    }
}