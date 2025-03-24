using Microsoft.AspNetCore.Mvc;
using UserService.Application.Contracts.UseCaseContracts;
using UserService.Application.Contracts.UseCaseContracts.Authentication;
using UserService.Application.DTO;
using UserService.Application.DTO.Authentication;
using UserService.Application.Validation;

namespace UserService.Presentation.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController(
    IRegisterUserUseCase registerUserUseCase,
    ILoginUseCase loginUseCase,
    IRefreshTokenForAuthUseCase refreshTokenForAuthUseCase)
    : ControllerBase
{
    [HttpPost("register")]
    [ValidationFilter<UserForRegistrationDto>]
    public async Task<IActionResult> RegisterUser(
        [FromBody] UserForRegistrationDto userForRegistration,
        CancellationToken cancellationToken)
    {
        var result = await registerUserUseCase.ExecuteAsync(userForRegistration, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.TryAddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }

        return StatusCode(201);
    }

    [HttpPost("login")]
    [ValidationFilter<UserForAuthenticationDto>]
    public async Task<IActionResult> Authenticate(
        [FromBody] UserForAuthenticationDto user,
        CancellationToken cancellationToken)
    {
        var tokenDto = await loginUseCase.ExecuteAsync(user, populateExp: true, cancellationToken);
        return Ok(tokenDto);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody]TokenDto tokenDto,
        CancellationToken cancellationToken)
    {
        var tokenDtoToReturn = await refreshTokenForAuthUseCase.ExecuteAsync(tokenDto);
        return Ok(tokenDtoToReturn);
    }
}