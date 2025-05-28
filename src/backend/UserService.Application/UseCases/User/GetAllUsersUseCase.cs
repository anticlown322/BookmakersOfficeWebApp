using AutoMapper;
using Domain.RequestFeatures;
using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.UseCases.User;
using UserService.Application.DTO;
using UserService.Application.DTO.User;
using UserService.Domain.RepositoryContracts;
using UserService.Domain.RequestFeatures;

namespace UserService.Application.UseCases.User;

public class GetAllUsersUseCase(
    IUsersRepository usersRepository,
    IMapper mapper,
    ILogger<GetAllUsersUseCase> logger)
    : IGetAllUsersUseCase
{
    public async Task<(IEnumerable<UserGetDto> users, MetaData metaData)> ExecuteAsync(
        UserParameters userParams,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting all users...");

        cancellationToken.ThrowIfCancellationRequested();

        var usersWithMetaData = await usersRepository.GetAllUsersAsync(userParams, cancellationToken);
        var usersDto = mapper.Map<IEnumerable<UserGetDto>>(usersWithMetaData);

        logger.LogInformation($"Successfully retrieved {usersDto.Count()} users");

        return (
            users: usersDto,
            metaData: usersWithMetaData.MetaData);
    }
}