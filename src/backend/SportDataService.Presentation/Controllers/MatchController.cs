using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Application.UseCases.Match;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Presentation.Controllers;

[Route("api/matches")]
[ApiController]
public class MatchController(
    IGetAllMatchesUseCase getAllMatchesUseCase)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMatches([FromQuery] MatchParameters matchParameters, CancellationToken cancellationToken)
    {
        var pagedResult = await getAllMatchesUseCase.ExecuteAsync(matchParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.matches);
    }

    // [HttpGet("{id:guid}")]
    // public async Task<IActionResult> GetMatchById(Guid id, CancellationToken cancellationToken)
    // {
    //     var eventToGet = await getMatchByIdUseCase.ExecuteAsync(id, cancellationToken);
    //
    //     return Ok(eventToGet);
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> CreateMatch([FromBody] MatchCreateDto matchCreateDto, CancellationToken cancellationToken)
    // {
    //     await CreateMatchUseCase.ExecuteAsync(matchCreateDto, cancellationToken);
    //
    //     return StatusCode(201);
    // }
    //
    // [HttpPut("{id:guid}")]
    // public async Task<IActionResult> UpdateMatch(Guid id, [FromBody] MatchUpdateDto matchUpdateDto, CancellationToken cancellationToken)
    // {
    //     await updateMatchUseCase.ExecuteAsync(id, cancellationToken);
    //
    //     return NoContent();
    // }
    //
    // [HttpDelete("{id:guid}")]
    // public async Task<IActionResult> DeleteMatch(Guid id, CancellationToken cancellationToken)
    // {
    //     await deleteMatchUseCase.ExecuteAsync(id, cancellationToken);
    //
    //     return NoContent();
    // }
}