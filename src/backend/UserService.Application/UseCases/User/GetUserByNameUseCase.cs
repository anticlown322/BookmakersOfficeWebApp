using AutoMapper;
using UserService.Application.Contracts.UseCases.User;
using UserService.Application.DTO.User;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.User;

public class GetUserByNameUseCase(
    IUsersRepository usersRepository,
    IMapper mapper)
    : IGetUserByNameUseCase
{
    public async Task<UserGetDto> ExecuteAsync(string username, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        var userDto = mapper.Map<UserGetDto>(userToGet);
        return userDto;
    }
}