using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.DTO.Account;
using UserService.Application.UseCases.Account;
using UserService.Application.Validation;

namespace UserService.Presentation.Controllers;

[Route("api/users/{username}/account")]
[ApiController]
public class AccountController(
    ISendConfirmationEmailUseCase sendConfirmationEmailUseCase,
    IConfirmEmailUseCase confirmEmailUseCase,
    ISendResetPasswordEmailUseCase sendResetPasswordEmailUseCase,
    IResetPasswordUseCase resetPasswordUseCase,
    IGetUserProfileUseCase getUserProfileUseCase,
    IUpdateUserProfileUseCase updateUserProfileUseCase)
    : ControllerBase
{
    [HttpGet("confirm-email")]
    public async Task<ActionResult> ConfirmEmail(
        [FromRoute] string username,
        CancellationToken cancellationToken)
    {
        await confirmEmailUseCase.ExecuteAsync(username, cancellationToken);

        return Ok();
    }

    [HttpPost("send-confirmation-email")]
    [Authorize(Policy= "AllUsers")]
    public async Task<ActionResult> SendConfirmationEmail(
        [FromRoute] string username,
        [FromServices] IHttpContextAccessor httpContextAccessor,
        CancellationToken cancellationToken)
    {
        var baseUrl = $"{httpContextAccessor.HttpContext!.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
        await sendConfirmationEmailUseCase.ExecuteAsync(username, baseUrl, cancellationToken);

        return Ok();
    }

    [HttpPost("send-reset-email")]
    public async Task<ActionResult> SendResetEmail(
        [FromRoute] string username,
        CancellationToken cancellationToken)
    {
        await sendResetPasswordEmailUseCase.ExecuteAsync(username, cancellationToken);

        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword(
        [FromRoute] string username,
        [FromBody] PasswordResetDto passwordResetDto,
        CancellationToken cancellationToken)
    {
        await resetPasswordUseCase.ExecuteAsync(username, passwordResetDto, cancellationToken);

        return Ok();
    }

    [HttpGet("profile")]
    [Authorize(Policy= "AllUsers")]
    public async Task<ActionResult> GetUserProfile(
        [FromRoute] string username,
        CancellationToken cancellationToken)
    {
        var result = await getUserProfileUseCase.ExecuteAsync(username, cancellationToken);

        return Ok(result);
    }

    [HttpPut("profile")]
    [Authorize(Policy= "AllUsers")]
    [ValidationFilter<UserProfileUpdateDto>]
    public async Task<ActionResult> UpdateUserProfile(
        [FromRoute] string username,
        [FromBody] UserProfileUpdateDto userProfileDto,
        CancellationToken cancellationToken)
    {
        await updateUserProfileUseCase.ExecuteAsync(username, userProfileDto, cancellationToken);
        return NoContent();
    }
}