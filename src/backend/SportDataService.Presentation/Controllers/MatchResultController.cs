using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SportDataService.Application.Contracts.UseCases.MatchResult;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Presentation.Controllers;

[Route("api/match-results")]
[ApiController]
public class MatchResultController(
    IGetAllMatchResultsUseCase getAllMatchResultsUseCase,
    IGetMatchResultByIdUseCase getMatchResultByIdUseCase,
    IGetMatchResultByResultIdUseCase getMatchResultByResultIdUseCase,
    ILogger<MatchResultController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMatchResults(
        [FromQuery] MatchResultParameters matchResultParameters,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting match results...");

        var pagedResult = await getAllMatchResultsUseCase.ExecuteAsync(matchResultParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.matchResults);
    }

    [HttpGet("by-id/{id}")]
    public async Task<IActionResult> GetMatchResultById(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting match result with id {id}...");

        var matchResultToGet = await getMatchResultByIdUseCase.ExecuteAsync(id, cancellationToken);

        return Ok(matchResultToGet);
    }

    [HttpGet("by-result-id/{matchResultId}")]
    public async Task<IActionResult> GetMatchResultByMatchResultId(string matchResultId, CancellationToken ct)
    {
        logger.LogInformation($"Getting match result with match result id {matchResultId}...");

        var matchResult = await getMatchResultByResultIdUseCase.ExecuteAsync(matchResultId, ct);

        return Ok(matchResult);
    }
}