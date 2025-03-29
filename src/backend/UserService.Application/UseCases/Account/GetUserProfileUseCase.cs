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
    public async Task<UserProfileForGetDto> ExecuteAsync(string username, CancellationToken cancellationToken)
    {
        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        var roles = await usersRepository.GetUserRolesAsync(userToGet, cancellationToken);

        var userProfileDto = mapper.Map<UserProfileForGetDto>(userToGet, opts => 
            opts.Items["Roles"] = roles.ToList());

        return userProfileDto;
    }
}