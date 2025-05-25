using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Presentation.Controllers;

[Route("api/matches")]
[ApiController]
public class MatchController(
    IGetAllMatchesUseCase getAllMatchesUseCase,
    IGetMatchByIdUseCase getMatchByIdUseCase,
    IGetMatchByMatchIdUseCase getMatchByMatchIdUseCase,
    ILogger<MatchController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMatches(
        [FromQuery] MatchParameters matchParameters,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting matches...");

        var pagedResult = await getAllMatchesUseCase.ExecuteAsync(matchParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.matches);
    }

    [HttpGet("by-id/{id}")]
    public async Task<IActionResult> GetMatchById(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting match by id {id}...");

        var matchToGet = await getMatchByIdUseCase.ExecuteAsync(id, cancellationToken);

        return Ok(matchToGet);
    }

    [HttpGet("by-match-id/{matchId}")]
    public async Task<IActionResult> GetMatchByMatchId(string matchId, CancellationToken ct)
    {
        logger.LogInformation($"Getting match by matchId {matchId}...");

        var match = await getMatchByMatchIdUseCase.ExecuteAsync(matchId, ct);
        return Ok(match);
    }
}