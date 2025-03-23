using Application.DTO.User;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Contracts.UseCaseContracts;
using UserService.Application.DTO;
using UserService.Application.Validation;

namespace UserService.Presentation.Controllers;

[Route("api/authentication")]
[ApiController]
public class AuthenticationController(
    IRegisterUserUseCase registerUserUseCase,
    ICreateTokenForAuthUseCase createTokenForAuthUseCase,
    IRefreshTokenForAuthUseCase refreshTokenForAuthUseCase)
    : ControllerBase
{
    [HttpPost]
    [ValidationFilter<UserForRegistrationDto>]
    public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
    {
        var result = await registerUserUseCase.ExecuteAsync(userForRegistration);

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
    public async Task<IActionResult> Authenticate([FromBody] UserForAuthenticationDto user)
    {
        var tokenDto = await createTokenForAuthUseCase.ExecuteAsync(user, populateExp: true);
        return Ok(tokenDto);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody]TokenDto tokenDto)
    {
        var tokenDtoToReturn = await refreshTokenForAuthUseCase.ExecuteAsync(tokenDto);
        return Ok(tokenDtoToReturn);
    }
}