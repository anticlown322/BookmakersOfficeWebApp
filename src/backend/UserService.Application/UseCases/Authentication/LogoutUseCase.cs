using UserService.Application.Contracts.UseCases.Authentication;
using UserService.Application.DTO.Authentication;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Authentication;

public class LogoutUseCase(IUsersRepository usersRepository) : ILogoutUseCase
{
    public async Task ExecuteAsync(UserForLogoutDto userForLogoutDto, bool populateExp, CancellationToken cancellationToken)
    {
        var user = await usersRepository.GetUserByNameAsync(userForLogoutDto.UserName, cancellationToken);

        if (user is null)
        {
            throw new UserNotFoundByNameException(userForLogoutDto.UserName);
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.Now.ToUniversalTime();

        await usersRepository.UpdateUserAsync(user, cancellationToken);
    }
}