using UserService.Application.DTO.User;

namespace UserService.Application.Contracts.UseCaseContracts.User;

public interface IGetUserByIdUseCase
{
    Task<UserForGetDto> ExecuteAsync(Guid userId, CancellationToken cancellationToken);
}