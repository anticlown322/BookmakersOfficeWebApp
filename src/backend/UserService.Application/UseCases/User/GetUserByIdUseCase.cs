using AutoMapper;
using UserService.Application.Contracts.UseCaseContracts.User;
using UserService.Application.DTO.User;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.User;

public class GetUserByIdUseCase(
    IUsersRepository usersRepository,
    IMapper mapper) : IGetUserByIdUseCase
{
    public async Task<UserForGetDto> ExecuteAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userToGet = await usersRepository.GetUserByIdAsync(userId, cancellationToken);
        if (userToGet is null)
        {
            throw new UserNotFoundByIdException(userId);
        }

        var userDto = mapper.Map<UserForGetDto>(userToGet);
        return userDto;
    }
}