using Microsoft.AspNetCore.Mvc;
using UserService.Application.Contracts.UseCases;
using UserService.Application.Contracts.UseCases.Authentication;
using UserService.Application.DTO;
using UserService.Application.DTO.Authentication;
using UserService.Application.Validation;

namespace UserService.Presentation.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController(
    IRegisterUserUseCase registerUserUseCase,
    ILoginUseCase loginUseCase,
    IRefreshTokenForAuthUseCase refreshTokenForAuthUseCase,
    ILogoutUseCase logoutUseCase)
    : ControllerBase
{
    [HttpPost("register")]
    [ValidationFilter<UserForRegistrationDto>]
    public async Task<IActionResult> Register(
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
    [ValidationFilter<UserForLoginDto>]
    public async Task<IActionResult> Login(
        [FromBody] UserForLoginDto user,
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
        var tokenDtoToReturn = await refreshTokenForAuthUseCase.ExecuteAsync(tokenDto, cancellationToken);
        return Ok(tokenDtoToReturn);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        UserForLogoutDto userForLogoutDto,
        CancellationToken cancellationToken)
    {
        await logoutUseCase.ExecuteAsync(userForLogoutDto, populateExp: true, cancellationToken);
        return Ok();
    }
}