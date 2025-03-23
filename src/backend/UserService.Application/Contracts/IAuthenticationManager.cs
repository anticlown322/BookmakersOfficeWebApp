using UserService.Application.DTO;
using UserService.Domain.Models;

namespace UserService.Application.Contracts;

public interface IAuthenticationManager
{
    Task<TokenDto> CreateTokens(User user, bool populateExp);
    Task<string> CreateAccessToken(User user);
}