using Application.DTO.User;
using UserService.Application.DTO;

namespace UserService.Application.Contracts.UseCaseContracts;

public interface IRefreshTokenForAuthUseCase
{
    Task<string> ExecuteAsync(TokenDto tokenDto);
}