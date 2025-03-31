using UserService.Application.Contracts.UseCases.Authentication;
using UserService.Application.DTO.Authentication;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Authentication;

public class LogoutUseCase(IUsersRepository usersRepository) : ILogoutUseCase
{
    public async Task ExecuteAsync(UserLogoutDto userLogoutDto, bool populateExp, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await usersRepository.GetUserByNameAsync(userLogoutDto.UserName, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundByNameException(userLogoutDto.UserName);
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.Now.ToUniversalTime();

        cancellationToken.ThrowIfCancellationRequested();

        await usersRepository.UpdateUserAsync(user, cancellationToken);
    }
}