using System.Security.Claims;
using System.Text.Json;
using BettingService.API.Utility;
using BettingService.BLL.DTO.Payout;
using BettingService.BLL.UseCases.Payments.Queries.GetAllPayouts;
using BettingService.BLL.UseCases.Payments.Queries.GetAllUserPayouts;
using BettingService.BLL.UseCases.Payments.Queries.GetPayoutByBetId;
using BettingService.BLL.UseCases.Payments.Queries.GetPayoutById;
using BettingService.BLL.UseCases.Payouts.Commands.RequestPayout;
using BettingService.BLL.Validation;
using BettingService.DAL.RequestFeatures.Params;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BettingService.API.Controllers;

[ApiController]
[Route("api/payouts")]
public class PayoutsController(
    IMediator mediator)
    : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.GamblerOnly)]
    [ValidationFilter<CreatePayoutDto>]
    public async Task<IActionResult> RequestPayout([FromBody] CreatePayoutDto payoutDto, CancellationToken cancellationToken)
    {
        var username = GetUsernameFromToken();
        var command = new RequestPayoutCommand(username, payoutDto);
        await mediator.Send(command, cancellationToken);

        return Created();
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.AdministratorOnly)]
    public async Task<IActionResult> GetAllPayouts(
        [FromQuery] PayoutParameters payoutParameters,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllPayoutsQuery(payoutParameters), cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.Data);
    }

    [HttpGet("my")]
    [Authorize(Policy = AuthorizationPolicies.GamblerOnly)]
    public async Task<IActionResult> GetAllUserPayouts(
        [FromQuery] PayoutParameters payoutParameters,
        CancellationToken cancellationToken)
    {
        var username = GetUsernameFromToken();
        var query = new GetAllUserPayoutsQuery(payoutParameters, username);
        var result = await mediator.Send(query, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.Data);
    }

    [HttpGet]
    [Route("{payoutId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdministratorOnly)]
    public async Task<IActionResult> GetPayoutById([FromRoute] Guid payoutId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPayoutByIdQuery(payoutId), cancellationToken);

        return Ok(result);
    }

    [HttpGet]
    [Route("by-bet-id/{betId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdministratorOnly)]
    public async Task<IActionResult> GetPayoutByBetId([FromRoute] Guid betId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetPayoutByBetIdQuery(betId), cancellationToken);

        return Ok(result);
    }

    private string GetUsernameFromToken()
    {
        var usernameClaim = User.FindFirst(ClaimTypes.Name) ??
                            User.FindFirst("name") ??
                            User.FindFirst("username");

        if (usernameClaim == null || string.IsNullOrWhiteSpace(usernameClaim.Value))
        {
            throw new InvalidOperationException("Username claim not found in token");
        }

        return usernameClaim.Value;
    }
}