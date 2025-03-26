using Microsoft.AspNetCore.Mvc;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.DTO.Authentication;
using UserService.Application.UseCases.Account;
using YamlDotNet.Core.Tokens;

namespace UserService.Presentation.Controllers;

[Route("api/users/{username}/account")]
[ApiController]
public class AccountController(
    ISendConfirmationEmailUseCase sendConfirmationEmailUseCase,
    IConfirmEmailUseCase confirmEmailUseCase)
    : ControllerBase
{
    [HttpGet("confirm-email")]
    public async Task<ActionResult> ConfirmEmail([FromRoute] string username, [FromQuery] string token, CancellationToken cancellationToken)
    {
        await confirmEmailUseCase.ExecuteAsync(username, token, cancellationToken);

        return Ok();
    }

    [HttpPost("send-confirmation-email")]
    public async Task<ActionResult> SendConfirmationEmail(
        [FromRoute] string username,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var baseUrl = $"{httpContextAccessor.HttpContext!.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
        await sendConfirmationEmailUseCase.ExecuteAsync(username, baseUrl, cancellationToken);

        return Ok();
    }
}