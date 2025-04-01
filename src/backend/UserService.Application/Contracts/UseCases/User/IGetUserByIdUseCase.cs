using UserService.Application.DTO.User;

namespace UserService.Application.Contracts.UseCases.User;

public interface IGetUserByIdUseCase
{
    Task<UserGetDto> ExecuteAsync(Guid userId, CancellationToken cancellationToken);
}