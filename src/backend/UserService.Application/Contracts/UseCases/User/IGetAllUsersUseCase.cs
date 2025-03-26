using Domain.RequestFeatures;
using UserService.Application.DTO;
using UserService.Application.DTO.User;
using UserService.Domain.RequestFeatures;

namespace UserService.Application.Contracts.UseCases.User;

public interface IGetAllUsersUseCase
{
    Task<(IEnumerable<UserForGetDto> users, MetaData metaData)>
        ExecuteAsync(UserParameters userParams, CancellationToken cancellationToken);
}