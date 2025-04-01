using AutoMapper;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Contracts.UseCases;
using UserService.Application.Contracts.UseCases.Authentication;
using UserService.Application.DTO.Authentication;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Authentication;

public class RegisterUserUseCase(
    IMapper mapper,
    IUsersRepository usersRepository)
    : IRegisterUserUseCase
{
    public async Task<IdentityResult> ExecuteAsync(
        UserRegistrationDto userRegistration,
        CancellationToken cancellationToken)
    {
        var existingUser = await usersRepository.GetUserByNameAsync(userRegistration.UserName, cancellationToken);
        if (existingUser != null)
        {
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
            throw new UserCanNonBeRegistered(result.Errors.First().Description);
        }

        user.Profile.UserId = user.Id;
        user.Balance.UserId = user.Id;
        user.Balance.LastUpdated = DateTime.UtcNow.ToUniversalTime();
        await usersRepository.UpdateUserAsync(user, cancellationToken);

        return result;
    }
}