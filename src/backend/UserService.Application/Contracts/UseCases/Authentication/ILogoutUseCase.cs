using UserService.Application.DTO.Authentication;

namespace UserService.Application.Contracts.UseCases.Authentication;

public interface ILogoutUseCase
{
    Task ExecuteAsync(UserForLogoutDto userForLogoutDto, bool populateExp, CancellationToken cancellationToken);
}