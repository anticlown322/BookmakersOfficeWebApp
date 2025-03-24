using UserService.Application.DTO;
using UserService.Application.DTO.Authentication;

namespace UserService.Application.Contracts.UseCaseContracts.Authentication;

public interface ILoginUseCase
{
    Task<TokenDto> ExecuteAsync(UserForAuthenticationDto userDto, bool populateExp, CancellationToken cancellationToken);
}