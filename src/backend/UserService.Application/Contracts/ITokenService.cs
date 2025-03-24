using UserService.Application.DTO;
using UserService.Application.DTO.Authentication;
using UserService.Domain.Models;

namespace UserService.Application.Contracts;

public interface ITokenService
{
    Task<TokenDto> CreateTokens(User user, bool populateExp);
    Task<string> CreateAccessToken(User user);
}