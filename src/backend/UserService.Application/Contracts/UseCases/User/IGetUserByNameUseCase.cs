using UserService.Application.DTO.User;

namespace UserService.Application.Contracts.UseCases.User;

public interface IGetUserByNameUseCase
{
    Task<UserGetDto> ExecuteAsync(string username, CancellationToken cancellationToken);
}