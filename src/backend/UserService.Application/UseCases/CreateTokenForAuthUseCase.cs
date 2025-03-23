using Microsoft.AspNetCore.Identity;
using UserService.Application.Contracts;
using UserService.Application.Contracts.UseCaseContracts;
using UserService.Application.DTO;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;

namespace UserService.Application.UseCases;

public class CreateTokenForAuthUseCase(
    IAuthenticationManager authenticationManager,
    UserManager<User> userManager) : ICreateTokenForAuthUseCase
{
    public async Task<TokenDto> ExecuteAsync(UserForAuthenticationDto userDto, bool populateExp)
    {
        var userEntity = await userManager.FindByNameAsync(userDto.UserName);
        if (userEntity == null || !await userManager.CheckPasswordAsync(userEntity, userDto.Password))
        {
            throw new InvalidCredentialsException(userDto.UserName, userDto.Password);
        }
        
        var tokenDto = await authenticationManager.CreateTokens(userEntity, populateExp);
        
        if(tokenDto.AccessToken is null)
            throw new TokenNotCreatedException(nameof(tokenDto.AccessToken));
        if(tokenDto.RefreshToken is null)
            throw new TokenNotCreatedException(nameof(tokenDto.RefreshToken));

        return tokenDto;
    }
}