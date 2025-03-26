using UserService.Application.DTO;
using UserService.Application.DTO.Authentication;

namespace UserService.Application.Contracts.UseCases.Authentication;

public interface ILoginUseCase
{
    Task<TokenDto> ExecuteAsync(UserForLoginDto userDto, bool populateExp, CancellationToken cancellationToken);
}