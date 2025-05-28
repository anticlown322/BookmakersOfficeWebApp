using AutoMapper;
using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.UseCases.User;
using UserService.Application.DTO.User;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.User;

public class GetUserByNameUseCase(
    IUsersRepository usersRepository,
    IMapper mapper,
    ILogger<GetUserByNameUseCase> logger)
    : IGetUserByNameUseCase
{
    public async Task<UserGetDto> ExecuteAsync(string username, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Attempting to retrieve user {username}");

        cancellationToken.ThrowIfCancellationRequested();

        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            logger.LogWarning($"User {username} not found");

            throw new UserNotFoundByNameException(username);
        }

        var userDto = mapper.Map<UserGetDto>(userToGet);

        logger.LogInformation($"Successfully retrieved {userDto.UserName}");

        return userDto;
    }
}