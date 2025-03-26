using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Contracts;
using UserService.Application.Contracts.Services;
using UserService.Application.Contracts.UseCases;
using UserService.Application.Contracts.UseCases.Authentication;
using UserService.Application.DTO;
using UserService.Application.DTO.Authentication;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;

namespace UserService.Application.UseCases.Authentication;

public class RefreshTokenForAuthUseCase(
    IOptions<JwtSettings> jwtSettings,
    IUsersRepository usersRepository,
    ITokenService tokenService) : IRefreshTokenForAuthUseCase
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public async Task<string> ExecuteAsync(TokenDto tokenDto, CancellationToken cancellationToken)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        var user = await usersRepository.GetUserByNameAsync(principal.Identity.Name, cancellationToken);

        if (user == null || user.RefreshToken != tokenDto.RefreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new RefreshTokenBadRequest();
        }

        var newAccessToken = await tokenService.CreateAccessToken(user);

        if (newAccessToken is null)
        {
            throw new TokenNotCreatedException(nameof(tokenDto.AccessToken));
        }

        return newAccessToken;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
            ValidateLifetime = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        var jwtSecurityToken = securityToken as JwtSecurityToken;
        if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(
            SecurityAlgorithms.HmacSha256,
            StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}