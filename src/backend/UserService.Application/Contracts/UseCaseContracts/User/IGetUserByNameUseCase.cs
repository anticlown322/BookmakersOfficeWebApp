using UserService.Application.DTO.User;

namespace UserService.Application.Contracts.UseCaseContracts.User;

public interface IGetUserByNameUseCase
{
    Task<UserForGetDto> ExecuteAsync(string username, CancellationToken cancellationToken);
}