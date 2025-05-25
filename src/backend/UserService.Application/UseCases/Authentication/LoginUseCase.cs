using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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
    SignInManager<Domain.Models.User> signInManager,
    ILogger<LoginUseCase> logger)
    : ILoginUseCase
{
    public async Task<TokensGetDto> ExecuteAsync(
        UserLoginDto userDto,
        bool populateExp,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Logging in for user {userDto.UserName}...");

        cancellationToken.ThrowIfCancellationRequested();

        var userEntity = await usersRepository.GetUserByNameAsync(userDto.UserName, cancellationToken);
        if (userEntity == null)
        {
            logger.LogWarning($"User {userDto.UserName} not found");

            throw new UserNotFoundByNameException(userDto.UserName);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var passwordCheckResult = await signInManager.CheckPasswordSignInAsync(userEntity, userDto.Password, false);
        if (!passwordCheckResult.Succeeded)
        {
            logger.LogWarning($"Password for user {userDto.UserName} is incorrect");

            throw new InvalidCredentialsException(userDto.UserName, userDto.Password);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var tokenDto = await tokenService.CreateTokens(userEntity, populateExp);

        if (tokenDto.AccessToken is null)
        {
            logger.LogWarning($"Access token for user {userDto.UserName} is empty");

            throw new TokenNotCreatedException(nameof(tokenDto.AccessToken));
        }

        if (tokenDto.RefreshToken is null)
        {
            logger.LogWarning($"Refresh token for user {userDto.UserName} is empty");

            throw new TokenNotCreatedException(nameof(tokenDto.RefreshToken));
        }

        logger.LogInformation($"Successfully created tokens for user {userDto.UserName}");

        return tokenDto;
    }
}