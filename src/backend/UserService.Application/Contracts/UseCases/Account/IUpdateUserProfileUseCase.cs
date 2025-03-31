using UserService.Application.DTO.Account;

namespace UserService.Application.Contracts.UseCases.Account;

public interface IUpdateUserProfileUseCase
{
    Task ExecuteAsync(string username, UserProfileUpdateDto userProfileUpdateDto, CancellationToken cancellationToken);
}