using UserService.Application.DTO.Account;

namespace UserService.Application.Contracts.UseCases.Account;

public interface IUpdateUserProfileUseCase
{
    Task ExecuteAsync(string username, UserProfileForUpdateDto userProfileForUpdateDto, CancellationToken cancellationToken);
}