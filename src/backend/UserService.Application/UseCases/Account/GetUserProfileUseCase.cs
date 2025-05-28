using AutoMapper;
using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.DTO.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class GetUserProfileUseCase(
    IUsersRepository usersRepository,
    IMapper mapper,
    ILogger<GetUserProfileUseCase> logger)
    : IGetUserProfileUseCase
{
    public async Task<UserProfileGetDto> ExecuteAsync(string username, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting user profile...");

        cancellationToken.ThrowIfCancellationRequested();

        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            logger.LogWarning($"User not found by username {username}");

            throw new UserNotFoundByNameException(username);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var roles = await usersRepository.GetUserRolesAsync(userToGet, cancellationToken);

        var userProfileDto = mapper.Map<UserProfileGetDto>(
            userToGet,
            opts =>
                opts.Items["Roles"] = roles.ToList());

        logger.LogInformation("Successfully retrieved user profile");

        return userProfileDto;
    }
}