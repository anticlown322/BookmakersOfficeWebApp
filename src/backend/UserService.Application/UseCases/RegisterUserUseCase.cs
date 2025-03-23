using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Application.DTO.User;
using UserService.Application.Contracts.UseCaseContracts;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;

namespace Application.UseCases.Authentication;

public class RegisterUserUseCase(
    IMapper mapper, 
    UserManager<User> userManager) : IRegisterUserUseCase
{
    public async Task<IdentityResult> ExecuteAsync(UserForRegistrationDto userForRegistration)
    {
        var existingUser = await userManager.FindByNameAsync(userForRegistration.UserName);
        if (existingUser != null)
        {
            throw new UserAlreadyExistsException(userForRegistration.UserName);
        }
        
        var user = mapper.Map<User>(userForRegistration);
        var result = await userManager.CreateAsync(user, userForRegistration.Password);
        
        if (result.Succeeded)
            await userManager.AddToRolesAsync(user, userForRegistration.Roles);
        
        return result;
    }
}