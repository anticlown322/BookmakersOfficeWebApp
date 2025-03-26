using UserService.Application.DTO.User;

namespace UserService.Application.Contracts.UseCases.User;

public interface IGetUserByIdUseCase
{
    Task<UserForGetDto> ExecuteAsync(Guid userId, CancellationToken cancellationToken);
}