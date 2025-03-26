using Microsoft.AspNetCore.Identity;
using UserService.Application.DTO.Authentication;

namespace UserService.Application.Contracts.UseCases.Authentication;

public interface IRegisterUserUseCase
{
    Task<IdentityResult> ExecuteAsync(UserForRegistrationDto userForRegistration, CancellationToken cancellationToken);
}