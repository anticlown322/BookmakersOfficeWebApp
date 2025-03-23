using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Contracts;
using UserService.Application.Contracts.UseCaseContracts;
using UserService.Application.DTO;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;

namespace UserService.Application.UseCases;

public class RefreshTokenForAuthUseCase(
    IConfiguration configuration,
    UserManager<User> userManager,
    IAuthenticationManager authenticationManager) : IRefreshTokenForAuthUseCase
{
    public async Task<string> ExecuteAsync(TokenDto tokenDto)
    {
        var principal = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
        var user = await userManager.FindByNameAsync(principal.Identity.Name);

        if (user == null || user.RefreshToken != tokenDto.RefreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.Now)
        {
            throw new RefreshTokenBadRequest();
        }

        var newAccessToken = await authenticationManager.CreateAccessToken(user);

        if (newAccessToken is null)
        {
            throw new TokenNotCreatedException(nameof(tokenDto.AccessToken));
        }

        return newAccessToken;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.GetSection("validIssuer").Value)),
            ValidateLifetime = true,
            ValidIssuer = jwtSettings["validIssuer"],
            ValidAudience = jwtSettings["validAudience"],
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

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