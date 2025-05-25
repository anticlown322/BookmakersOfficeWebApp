using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Contracts;
using UserService.Application.Contracts.Services;
using UserService.Application.DTO;
using UserService.Application.DTO.Authentication;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;

namespace UserService.Infrastructure.Services;

public class TokenService(
    IUsersRepository usersRepository,
    IOptions<JwtSettings> jwtSettings,
    ILogger<TokenService> logger)
    : ITokenService
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private User? _user;

    public async Task<TokensGetDto> CreateTokens(User user, bool populateExp)
    {
        logger.LogInformation("Creating new tokens...");

        _user = user;

        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);
        var refreshToken = GenerateRefreshToken();

        _user.RefreshToken = refreshToken;
        if (populateExp)
        {
            _user.RefreshTokenExpiryTime = DateTime.Now.AddDays(1).ToUniversalTime();
        }

        logger.LogInformation("Updating tokens in database...");

        await usersRepository.UpdateUserAsync(_user);

        logger.LogInformation("Tokens updated");

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        logger.LogInformation("Successfully created new tokens...");

        return new TokensGetDto(accessToken, refreshToken);
    }

    public async Task<string> CreateAccessToken(User user)
    {
        logger.LogInformation("Creating new access token...");

        _user = user;
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        logger.LogInformation("Updating access token in database...");

        await usersRepository.UpdateUserAsync(_user);

        logger.LogInformation("Successfully updated access token in database...");

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        logger.LogInformation("Successfully created new access token...");

        return accessToken;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    private SigningCredentials GetSigningCredentials()
    {
        logger.LogInformation("Getting signing credentials...");

        var secretKey = _jwtSettings.SecretKey;
        var key = Encoding.UTF8.GetBytes(secretKey);
        var secret = new SymmetricSecurityKey(key);

        logger.LogInformation("Successfully got signing credentials...");

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims()
    {
        logger.LogInformation("Getting claims...");

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, _user.UserName),
        };

        var roles = await usersRepository.GetUserRolesAsync(_user);

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        logger.LogInformation("Successfully retrieved claims...");

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        logger.LogInformation("Generating token options...");

        var tokenOptions = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: signingCredentials);

        logger.LogInformation("Token options generation complete...");

        return tokenOptions;
    }
}