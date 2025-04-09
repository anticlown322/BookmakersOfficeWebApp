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
    IRefreshTournaments refreshTournaments,
    IGetAllTournamentsUseCase getAllTournamentsUseCase,
    IGetTournamentByIdUseCase getTournamentByIdUseCase,
    IGetTournamentByTournamentIdUseCase getTournamentByTournamentIdUseCase)
    : ControllerBase
{
    [HttpPost]
    // [Authorize(Policy = AuthorizationPolicies.AdministratorOnly)]
    public async Task<IActionResult> RefreshTournaments(CancellationToken cancellationToken)
    {
        await refreshTournaments.ExecuteAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetTournaments([FromQuery] TournamentParameters tournamentParameters, CancellationToken cancellationToken)
    {
        var pagedResult = await getAllTournamentsUseCase.ExecuteAsync(tournamentParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.tournaments);
    }

    [HttpGet("by-id/{id}")]
    public async Task<IActionResult> GetTournamentById(string id, CancellationToken cancellationToken)
    {
        var tournamentToGet = await getTournamentByIdUseCase.ExecuteAsync(id, cancellationToken);

        return Ok(tournamentToGet);
    }

    [HttpGet("by-tournament-id/{tournamentId}")]
    public async Task<IActionResult> GetTournamentByTournamentId(string tournamentId, CancellationToken ct)
    {
        var tournament = await getTournamentByTournamentIdUseCase.ExecuteAsync(tournamentId, ct);
        return Ok(tournament);
    }
}