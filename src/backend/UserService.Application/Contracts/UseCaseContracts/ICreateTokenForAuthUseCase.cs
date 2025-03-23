using UserService.Application.DTO;

namespace UserService.Application.Contracts.UseCaseContracts;

public interface ICreateTokenForAuthUseCase
{
    Task<TokenDto> ExecuteAsync(UserForAuthenticationDto userDto, bool populateExp);
}