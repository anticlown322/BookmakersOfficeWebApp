using Microsoft.AspNetCore.Identity;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.DTO.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class ResetPasswordUseCase(IUsersRepository usersRepository, UserManager<Domain.Models.User> userManager) : IResetPasswordUseCase
{
    public async Task ExecuteAsync(string username, PasswordResetDto passwordResetDto, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        var result = await userManager.ResetPasswordAsync(user, passwordResetDto.token, passwordResetDto.newPassword);
        if (result.Errors.Any())
        {
            var error = result.Errors.FirstOrDefault();
            throw new PasswordCanNotBeReset(error.Description);
        }
    }
}