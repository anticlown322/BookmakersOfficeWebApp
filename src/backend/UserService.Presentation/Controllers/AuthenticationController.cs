using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Contracts.UseCases;
using UserService.Application.Contracts.UseCases.Authentication;
using UserService.Application.DTO;
using UserService.Application.DTO.Authentication;
using UserService.Application.Validation;
using UserService.Presentation.Utility;

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
    [ValidationFilter<UserRegistrationDto>]
    public async Task<IActionResult> Register(
        [FromBody] UserRegistrationDto userRegistration,
        CancellationToken cancellationToken)
    {
        var result = await registerUserUseCase.ExecuteAsync(userRegistration, cancellationToken);

        return StatusCode(201);
    }

    [HttpPost("login")]
    [ValidationFilter<UserLoginDto>]
    public async Task<IActionResult> Login(
        [FromBody] UserLoginDto user,
        CancellationToken cancellationToken)
    {
        var tokenDto = await loginUseCase.ExecuteAsync(user, populateExp: true, cancellationToken);

        return Ok(tokenDto);
    }

    [HttpPost("refresh")]
    [Authorize(Policy= AuthorizationPolicies.AllUsers)]
    [ValidationFilter<TokensRefreshDto>]
    public async Task<IActionResult> Refresh(
        [FromBody] TokensRefreshDto tokensGetDto,
        CancellationToken cancellationToken)
    {
        var tokenDtoToReturn = await refreshTokenForAuthUseCase.ExecuteAsync(tokensGetDto, cancellationToken);

        return Ok(tokenDtoToReturn);
    }

    [HttpPost("logout")]
    [Authorize(Policy= AuthorizationPolicies.AllUsers)]
    [ValidationFilter<UserLogoutDto>]
    public async Task<IActionResult> Logout(
        [FromBody] UserLogoutDto userLogoutDto,
        CancellationToken cancellationToken)
    {
        await logoutUseCase.ExecuteAsync(userLogoutDto, populateExp: true, cancellationToken);

        return Ok();
    }
}