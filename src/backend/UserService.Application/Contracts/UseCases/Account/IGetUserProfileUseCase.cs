using UserService.Application.DTO.Account;

namespace UserService.Application.Contracts.UseCases.Account;

public interface IGetUserProfileUseCase
{
    public Task<UserProfileForGetDto> ExecuteAsync(string username, CancellationToken cancellationToken);
}