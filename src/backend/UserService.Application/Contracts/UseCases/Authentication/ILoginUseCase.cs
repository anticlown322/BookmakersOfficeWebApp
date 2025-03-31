using UserService.Application.DTO;
using UserService.Application.DTO.Authentication;

namespace UserService.Application.Contracts.UseCases.Authentication;

public interface ILoginUseCase
{
    Task<TokensGetDto> ExecuteAsync(UserLoginDto userDto, bool populateExp, CancellationToken cancellationToken);
}