using BettingService.BLL.UseCases.Bets.Commands.UpdateActiveBets;
using BettingService.BLL.UseCases.Bets.Commands.UpdatePendingBets;
using BettingService.BLL.UseCases.Bets.Queries.GetAllUserBets;
using BettingService.BLL.Validation;
using BettingService.BLL.Validation.Validators;

namespace BettingService.API.Controllers;

using System.Security.Claims;
using System.Text.Json;
using BettingService.API.Utility;
using BettingService.BLL.DTO.Bet;
using BettingService.BLL.UseCases.Bets.Commands.PlaceBet;
using BettingService.BLL.UseCases.Bets.Queries.GetAllBets;
using BettingService.BLL.UseCases.Bets.Queries.GetBetById;
using BettingService.DAL.RequestFeatures.Params;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/bets")]
public class BetsController(IMediator mediator)
    : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.GamblerOnly)]
    public async Task<IActionResult> PlaceBet([FromBody] PlaceBetDto placeBetDto, CancellationToken cancellationToken)
    {
        var username = GetUsernameFromToken();
        var command = new PlaceBetCommand(username, placeBetDto);
        var result = await mediator.Send(command, cancellationToken);

        return Created(result.BetId.ToString(), result.Status.ToString());
    }

    [HttpPost("pending")]
    [Authorize(Policy = AuthorizationPolicies.AdministratorOnly)]
    public async Task<IActionResult> UpdatePendingBets(CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdatePendingBetsCommand(), cancellationToken);

        return Ok();
    }

    [HttpPost("active")]
    [Authorize(Policy = AuthorizationPolicies.AdministratorOnly)]
    public async Task<IActionResult> UpdateActiveBets(CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateActiveBetsCommand(), cancellationToken);

        return Ok();
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.AdministratorOnly)]
    public async Task<IActionResult> GetAllBets(
        [FromQuery] BetParameters betParameters,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllBetsQuery(betParameters), cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.Data);
    }

    [HttpGet("my")]
    [Authorize(Policy = AuthorizationPolicies.GamblerOnly)]
    public async Task<IActionResult> GetAllUserBets(
        [FromQuery] BetParameters betParameters,
        CancellationToken cancellationToken)
    {
        var username = GetUsernameFromToken();
        var query = new GetAllUserBetsQuery(betParameters, username);
        var result = await mediator.Send(query, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(result.MetaData));

        return Ok(result.Data);
    }

    [HttpGet]
    [Route("{betId:guid}")]
    [Authorize(Policy = AuthorizationPolicies.AdministratorOnly)]
    public async Task<IActionResult> GetBetById([FromRoute] Guid betId, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetBetByIdQuery(betId), cancellationToken);

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