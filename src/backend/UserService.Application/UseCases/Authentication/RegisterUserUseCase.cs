using AutoMapper;
using Microsoft.AspNetCore.Identity;
using UserService.Application.Contracts.UseCaseContracts;
using UserService.Application.Contracts.UseCaseContracts.Authentication;
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
        var result = await usersRepository.CreateUserAsync(user, userForRegistration.Password, userForRegistration.Roles, cancellationToken);

        if (result.Errors.Any())
        {
            var error = result.Errors.FirstOrDefault();
            throw new UserCanNonBeRegistered(error.Description);
        }

        return result;
    }
}