using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Domain.RequestFeatures.Params;
using SportDataService.Presentation.Utility;

namespace SportDataService.Presentation.Controllers;

[Route("api/tournaments")]
[ApiController]
public class TournamentController(
    IRefreshTournamentsUseCase refreshTournamentsUseCase,
    IGetAllTournamentsUseCase getAllTournamentsUseCase,
    IGetTournamentByIdUseCase getTournamentByIdUseCase,
    IGetTournamentByTournamentIdUseCase getTournamentByTournamentIdUseCase,
    ILogger<TournamentController> logger)
    : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.AdministratorOnly)]
    public async Task<IActionResult> RefreshTournaments(CancellationToken cancellationToken)
    {
        logger.LogInformation("Refreshing tournaments...");

        await refreshTournamentsUseCase.ExecuteAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetTournaments(
        [FromQuery] TournamentParameters tournamentParameters,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting tournaments...");

        var pagedResult = await getAllTournamentsUseCase.ExecuteAsync(tournamentParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.tournaments);
    }

    [HttpGet("by-id/{id}")]
    public async Task<IActionResult> GetTournamentById(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting tournament with id {id}...");

        var tournamentToGet = await getTournamentByIdUseCase.ExecuteAsync(id, cancellationToken);

        return Ok(tournamentToGet);
    }

    [HttpGet("by-tournament-id/{tournamentId}")]
    public async Task<IActionResult> GetTournamentByTournamentId(string tournamentId, CancellationToken ct)
    {
        logger.LogInformation($"Getting tournament with tournament id {tournamentId}...");

        var tournament = await getTournamentByTournamentIdUseCase.ExecuteAsync(tournamentId, ct);

        return Ok(tournament);
    }
}