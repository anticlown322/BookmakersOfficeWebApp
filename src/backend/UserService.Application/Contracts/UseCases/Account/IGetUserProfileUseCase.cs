using UserService.Application.DTO.Account;

namespace UserService.Application.Contracts.UseCases.Account;

public interface IGetUserProfileUseCase
{
    public Task<UserProfileGetDto> ExecuteAsync(string username, CancellationToken cancellationToken);
}