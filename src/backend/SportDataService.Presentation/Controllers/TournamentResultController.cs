using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportDataService.Application.Contracts.UseCases.TournamentResult;
using SportDataService.Domain.RequestFeatures.Params;
using SportDataService.Presentation.Utility;

namespace SportDataService.Presentation.Controllers;

[Route("api/tournament-results")]
[ApiController]
public class TournamentResultController(
    IRefreshTournamentResultsUseCase refreshTournamentResultsUseCase,
    IGetAllTournamentResultsUseCase getAllTournamentResultsUseCase,
    IGetTournamentResultByIdUseCase getTournamentResultByIdUseCase,
    IGetTournamentResultByResultIdUseCase getTournamentResultByResultIdUseCase)
    : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.AdministratorOnly)]
    public async Task<IActionResult> RefreshTournamentResults(CancellationToken cancellationToken)
    {
        await refreshTournamentResultsUseCase.ExecuteAsync(cancellationToken);

        return NoContent();
    }

    [HttpGet]
    public async Task<IActionResult> GetTournamentResults([FromQuery] TournamentResultParameters tournamentResultParameters, CancellationToken cancellationToken)
    {
        var pagedResult = await getAllTournamentResultsUseCase.ExecuteAsync(tournamentResultParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.tournamentResults);
    }

    [HttpGet("by-id/{id}")]
    public async Task<IActionResult> GetTournamentResultById(string id, CancellationToken cancellationToken)
    {
        var tournamentResultToGet = await getTournamentResultByIdUseCase.ExecuteAsync(id, cancellationToken);

        return Ok(tournamentResultToGet);
    }

    [HttpGet("by-result-id/{tournamentResultId}")]
    public async Task<IActionResult> GetTournamentResultByTournamentResultId(string tournamentResultId, CancellationToken ct)
    {
        var tournamentResult = await getTournamentResultByResultIdUseCase.ExecuteAsync(tournamentResultId, ct);
        return Ok(tournamentResult);
    }
}