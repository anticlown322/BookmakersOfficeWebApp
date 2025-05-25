using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.UseCases;
using UserService.Application.Contracts.UseCases.Authentication;
using UserService.Application.DTO.Authentication;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Authentication;

public class RegisterUserUseCase(
    IMapper mapper,
    IUsersRepository usersRepository,
    ILogger<RegisterUserUseCase> logger)
    : IRegisterUserUseCase
{
    public async Task<IdentityResult> ExecuteAsync(
        UserRegistrationDto userRegistration,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Registering user {userRegistration.UserName}");

        var existingUser = await usersRepository.GetUserByNameAsync(userRegistration.UserName, cancellationToken);
        if (existingUser != null)
        {
            logger.LogWarning($"User {existingUser.UserName} is already registered");

            throw new UserAlreadyExistsException(userRegistration.UserName);
        }

        var user = mapper.Map<Domain.Models.User>(userRegistration);

        var result = await usersRepository.CreateUserAsync(
            user,
            userRegistration.Password,
            userRegistration.Roles ?? new List<string>(),
            cancellationToken);

        if (!result.Succeeded)
        {
            logger.LogWarning($"Errors while registering {userRegistration.UserName}:  {result.Errors}");

            throw new UserCanNonBeRegistered(result.Errors.First().Description);
        }

        user.Profile.UserId = user.Id;
        user.Balance.UserId = user.Id;
        user.Balance.LastUpdated = DateTime.UtcNow.ToUniversalTime();
        await usersRepository.UpdateUserAsync(user, cancellationToken);

        logger.LogInformation($"User {user.UserName} successfully created");

        return result;
    }
}