using Microsoft.AspNetCore.Identity;
using UserService.Application.DTO.Authentication;

namespace UserService.Application.Contracts.UseCaseContracts.Authentication;

public interface IRegisterUserUseCase
{
    Task<IdentityResult> ExecuteAsync(UserForRegistrationDto userForRegistration, CancellationToken cancellationToken);
}