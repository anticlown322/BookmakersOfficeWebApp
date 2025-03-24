using Microsoft.AspNetCore.Identity;
using UserService.Application.Contracts;
using UserService.Application.Contracts.UseCaseContracts;
using UserService.Application.Contracts.UseCaseContracts.Authentication;
using UserService.Application.DTO;
using UserService.Application.DTO.Authentication;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Authentication;

public class LoginUseCase(
    ITokenService tokenService,
    IUsersRepository usersRepository,
    SignInManager<Domain.Models.User> signInManager)
    : ILoginUseCase
{
    public async Task<TokenDto> ExecuteAsync(
        UserForAuthenticationDto userDto,
        bool populateExp,
        CancellationToken cancellationToken)
    {
        var userEntity = await usersRepository.GetUserByNameAsync(userDto.UserName, cancellationToken);
        var passwordCheckResult = await signInManager.CheckPasswordSignInAsync(userEntity, userDto.Password, false);
        if (userEntity == null || !passwordCheckResult.Succeeded)
        {
            throw new InvalidCredentialsException(userDto.UserName, userDto.Password);
        }

        var tokenDto = await tokenService.CreateTokens(userEntity, populateExp);

        if (tokenDto.AccessToken is null)
        {
            throw new TokenNotCreatedException(nameof(tokenDto.AccessToken));
        }

        if (tokenDto.RefreshToken is null)
        {
            throw new TokenNotCreatedException(nameof(tokenDto.RefreshToken));
        }

        return tokenDto;
    }
}