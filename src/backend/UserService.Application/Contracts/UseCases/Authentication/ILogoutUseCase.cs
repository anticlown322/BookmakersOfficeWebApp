using UserService.Application.DTO.Authentication;

namespace UserService.Application.Contracts.UseCases.Authentication;

public interface ILogoutUseCase
{
    Task ExecuteAsync(UserLogoutDto userLogoutDto, bool populateExp, CancellationToken cancellationToken);
}