using AutoMapper;
using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.DTO.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class UpdateUserProfileUseCase(
    IUsersRepository usersRepository,
    IMapper mapper,
    ILogger<UpdateUserProfileUseCase> logger)
    : IUpdateUserProfileUseCase
{
    public async Task ExecuteAsync(
        string username,
        UserProfileUpdateDto userProfileUpdateDto,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Updating user profile for {username}");

        cancellationToken.ThrowIfCancellationRequested();

        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            logger.LogWarning($"User with username {username} was not found");

            throw new UserNotFoundByNameException(username);
        }

        mapper.Map(userProfileUpdateDto, userToGet);

        cancellationToken.ThrowIfCancellationRequested();

        await usersRepository.UpdateUserAsync(userToGet, cancellationToken);

        logger.LogInformation($"Successfully updated {username} profile");
    }
}