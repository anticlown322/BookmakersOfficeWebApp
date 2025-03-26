using UserService.Application.DTO;
using UserService.Application.DTO.Authentication;

namespace UserService.Application.Contracts.UseCases.Authentication;

public interface IRefreshTokenForAuthUseCase
{
    Task<string> ExecuteAsync(TokenDto tokenDto, CancellationToken cancellationToken);
}