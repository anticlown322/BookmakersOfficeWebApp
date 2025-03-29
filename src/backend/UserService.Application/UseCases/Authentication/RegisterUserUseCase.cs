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
        UserForRegistrationDto userForRegistration,
        CancellationToken cancellationToken)
    {
        var existingUser = await usersRepository.GetUserByNameAsync(userForRegistration.UserName, cancellationToken);
        if (existingUser != null)
        {
            throw new UserAlreadyExistsException(userForRegistration.UserName);
        }

        var user = mapper.Map<Domain.Models.User>(userForRegistration);

        var result = await usersRepository.CreateUserAsync(
            user,
            userForRegistration.Password,
            userForRegistration.Roles ?? new List<string>(),
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