using UserService.Application.DTO.Authentication;
using UserService.Domain.Models;

namespace UserService.Application.Contracts.Services;

public interface ITokenService
{
    Task<TokensGetDto> CreateTokens(User user, bool populateExp);
    Task<string> CreateAccessToken(User user);
}