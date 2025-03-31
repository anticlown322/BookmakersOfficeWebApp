using Microsoft.AspNetCore.Identity;
using UserService.Application.DTO.Authentication;

namespace UserService.Application.Contracts.UseCases.Authentication;

public interface IRegisterUserUseCase
{
    Task<IdentityResult> ExecuteAsync(UserRegistrationDto userRegistration, CancellationToken cancellationToken);
}