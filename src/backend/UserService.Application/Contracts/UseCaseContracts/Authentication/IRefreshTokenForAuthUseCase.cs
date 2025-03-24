using UserService.Application.DTO;
using UserService.Application.DTO.Authentication;

namespace UserService.Application.Contracts.UseCaseContracts.Authentication;

public interface IRefreshTokenForAuthUseCase
{
    Task<string> ExecuteAsync(TokenDto tokenDto);
}