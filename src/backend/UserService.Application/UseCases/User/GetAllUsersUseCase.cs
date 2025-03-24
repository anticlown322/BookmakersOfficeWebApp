using AutoMapper;
using Domain.RequestFeatures;
using UserService.Application.Contracts.UseCaseContracts.User;
using UserService.Application.DTO;
using UserService.Application.DTO.User;
using UserService.Domain.RepositoryContracts;
using UserService.Domain.RequestFeatures;

namespace UserService.Application.UseCases.User;

public class GetAllUsersUseCase(
    IUsersRepository usersRepository,
    IMapper mapper)
    : IGetAllUsersUseCase
{
    public async Task<(IEnumerable<UserForGetDto> users, MetaData metaData)> ExecuteAsync(UserParameters userParams, CancellationToken cancellationToken)
    {
        var usersWithMetaData = await usersRepository.GetAllUsersAsync(userParams, cancellationToken);
        var usersDto = mapper.Map<IEnumerable<UserForGetDto>>(usersWithMetaData);

        return (
            users: usersDto,
            metaData: usersWithMetaData.MetaData);
    }
}