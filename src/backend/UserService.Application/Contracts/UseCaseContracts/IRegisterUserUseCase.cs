using Application.DTO.User;
using Microsoft.AspNetCore.Identity;

namespace UserService.Application.Contracts.UseCaseContracts;

public interface IRegisterUserUseCase
{
    Task<IdentityResult> ExecuteAsync(UserForRegistrationDto userForRegistration);
}