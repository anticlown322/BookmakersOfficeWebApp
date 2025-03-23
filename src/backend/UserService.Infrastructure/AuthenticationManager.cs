using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Contracts;
using UserService.Application.DTO;
using UserService.Domain.Models;
using UserService.Infrastructure.AppSettings;

namespace UserService.Infrastructure;

public class AuthenticationManager(
    UserManager<User> userManager,
    IOptions<JwtSettings> jwtSettings)
    : IAuthenticationManager
{
    private readonly JwtSettings _jwtSettings = jwtSettings.Value;
    private User? _user;

    public async Task<TokenDto> CreateTokens(User user, bool populateExp)
    {
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

        await userManager.UpdateAsync(_user);

        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        return new TokenDto(accessToken, refreshToken);
    }

    public async Task<string> CreateAccessToken(User user)
    {
        _user = user;
        var signingCredentials = GetSigningCredentials();
        var claims = await GetClaims();
        var tokenOptions = GenerateTokenOptions(signingCredentials, claims);

        await userManager.UpdateAsync(_user);
        var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
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
        var secretKey = _jwtSettings.SecretKey;
        var key = Encoding.UTF8.GetBytes(secretKey);
        var secret = new SymmetricSecurityKey(key);

        return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
    }

    private async Task<List<Claim>> GetClaims()
    {
        var claims = new List<Claim>
        {
            new (ClaimTypes.Name, _user.UserName),
        };

        var roles = await userManager.GetRolesAsync(_user);

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return claims;
    }

    private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
    {
        var tokenOptions = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: signingCredentials);

        return tokenOptions;
    }
}