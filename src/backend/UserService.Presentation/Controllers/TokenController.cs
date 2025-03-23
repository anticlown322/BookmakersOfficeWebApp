using Microsoft.AspNetCore.Mvc;
using Application.DTO.User;
using UserService.Application.Contracts.UseCaseContracts;
using UserService.Application.DTO;

namespace Presentation.Controllers;

[Route("api/token")]
[ApiController]
public class TokenController(
    IRefreshTokenForAuthUseCase refreshTokenForAuthUseCase) 
    : ControllerBase
{
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody]TokenDto tokenDto)
    {
        var tokenDtoToReturn = await refreshTokenForAuthUseCase
            .ExecuteAsync(tokenDto);

        return Ok(tokenDtoToReturn);
    }
}