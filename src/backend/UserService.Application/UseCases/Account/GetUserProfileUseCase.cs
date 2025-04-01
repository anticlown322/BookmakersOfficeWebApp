using AutoMapper;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.DTO.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class GetUserProfileUseCase(
    IUsersRepository usersRepository,
    IMapper mapper)
    : IGetUserProfileUseCase
{
    public async Task<UserProfileGetDto> ExecuteAsync(string username, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var roles = await usersRepository.GetUserRolesAsync(userToGet, cancellationToken);

        var userProfileDto = mapper.Map<UserProfileGetDto>(userToGet, opts => 
            opts.Items["Roles"] = roles.ToList());

        return userProfileDto;
    }
}