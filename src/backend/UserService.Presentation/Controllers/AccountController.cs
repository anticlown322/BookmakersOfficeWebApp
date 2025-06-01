using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.DTO.Account;
using UserService.Application.UseCases.Account;
using UserService.Application.Validation;
using UserService.Domain.Models;
using UserService.Presentation.Utility;

namespace UserService.Presentation.Controllers;

[Route("api/users/{username}/account")]
[ApiController]
public class AccountController(
    ISendConfirmationEmailUseCase sendConfirmationEmailUseCase,
    IConfirmEmailUseCase confirmEmailUseCase,
    ISendResetPasswordEmailUseCase sendResetPasswordEmailUseCase,
    IResetPasswordUseCase resetPasswordUseCase,
    IGetUserProfileUseCase getUserProfileUseCase,
    IUpdateUserProfileUseCase updateUserProfileUseCase,
    ILogger<AccountController> logger)
    : ControllerBase
{
    [HttpGet("confirm-email")]
    public async Task<ActionResult> ConfirmEmail(
        [FromRoute] string username,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Confirm email for user {username}...");

        await confirmEmailUseCase.ExecuteAsync(username, cancellationToken);

        return Ok();
    }

    [HttpPost("send-confirmation-email")]
    [Authorize(Policy = AuthorizationPolicies.AllUsers)]
    public async Task<ActionResult> SendConfirmationEmail(
        [FromRoute] string username,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Send confirmation email for user {username}...");

        await sendConfirmationEmailUseCase.ExecuteAsync(username, cancellationToken);

        return Ok();
    }

    [HttpPost("send-reset-email")]
    public async Task<ActionResult> SendResetEmail(
        [FromRoute] string username,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Send password reset email for user {username}...");

        await sendResetPasswordEmailUseCase.ExecuteAsync(username, cancellationToken);

        return Ok();
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword(
        [FromRoute] string username,
        [FromBody] PasswordResetDto passwordResetDto,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Reset password for user {username}...");

        await resetPasswordUseCase.ExecuteAsync(username, passwordResetDto, cancellationToken);

        return Ok();
    }

    [HttpGet("profile")]
    [Authorize(Policy = AuthorizationPolicies.AllUsers)]
    public async Task<ActionResult> GetUserProfile(
        [FromRoute] string username,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Get profile of user {username}...");

        var result = await getUserProfileUseCase.ExecuteAsync(username, cancellationToken);

        return Ok(result);
    }

    [HttpPut("profile")]
    [Authorize(Policy = AuthorizationPolicies.AllUsers)]
    [ValidationFilter<UserProfileUpdateDto>]
    public async Task<ActionResult> UpdateUserProfile(
        [FromRoute] string username,
        [FromBody] UserProfileUpdateDto userProfileDto,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Update profile of user {username}...");

        await updateUserProfileUseCase.ExecuteAsync(username, userProfileDto, cancellationToken);
        return NoContent();
    }
}