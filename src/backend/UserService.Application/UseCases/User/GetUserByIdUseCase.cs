using AutoMapper;
using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.UseCases.User;
using UserService.Application.DTO.User;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.User;

public class GetUserByIdUseCase(
    IUsersRepository usersRepository,
    IMapper mapper,
    ILogger<GetUserByIdUseCase> logger)
    : IGetUserByIdUseCase
{
    public async Task<UserGetDto> ExecuteAsync(Guid userId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Attempting to retrieve user by id {userId}");

        cancellationToken.ThrowIfCancellationRequested();

        var userToGet = await usersRepository.GetUserByIdAsync(userId, cancellationToken);
        if (userToGet is null)
        {
            logger.LogWarning($"User with id {userId} not found");

            throw new UserNotFoundByIdException(userId);
        }

        var userDto = mapper.Map<UserGetDto>(userToGet);

        logger.LogInformation($"Successfully retrieved {userDto.UserName} with id {userId}");

        return userDto;
    }
}