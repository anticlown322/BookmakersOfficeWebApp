using Microsoft.AspNetCore.Identity;
using UserService.Application.Contracts;
using UserService.Application.Contracts.Services;
using UserService.Application.Contracts.UseCases;
using UserService.Application.Contracts.UseCases.Authentication;
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
    public async Task<TokensGetDto> ExecuteAsync(
        UserLoginDto userDto,
        bool populateExp,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var userEntity = await usersRepository.GetUserByNameAsync(userDto.UserName, cancellationToken);
        if (userEntity == null)
        {
            throw new UserNotFoundByNameException(userDto.UserName);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var passwordCheckResult = await signInManager.CheckPasswordSignInAsync(userEntity, userDto.Password, false);
        if (!passwordCheckResult.Succeeded)
        {
            throw new InvalidCredentialsException(userDto.UserName, userDto.Password);
        }

        cancellationToken.ThrowIfCancellationRequested();

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