using AutoMapper;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.DTO.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Account;

public class UpdateUserProfileUseCase(
    IUsersRepository usersRepository,
    IMapper mapper
    ) : IUpdateUserProfileUseCase
{
    public async Task ExecuteAsync(string username, UserProfileForUpdateDto userProfileForUpdateDto, CancellationToken cancellationToken)
    {
        var userToGet = await usersRepository.GetUserByNameAsync(username, cancellationToken);
        if (userToGet is null)
        {
            throw new UserNotFoundByNameException(username);
        }

        mapper.Map(userProfileForUpdateDto, userToGet);
        await usersRepository.UpdateUserAsync(userToGet, cancellationToken);
    }
}