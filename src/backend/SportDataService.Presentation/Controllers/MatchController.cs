using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Application.DTO.Match;
using SportDataService.Application.UseCases.Match;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Presentation.Controllers;

[Route("api/matches")]
[ApiController]
public class MatchController(
    IGetAllMatchesUseCase getAllMatchesUseCase,
    IGetMatchByIdUseCase getMatchByIdUseCase,
    ICreateMatchUseCase createMatchUseCase,
    IUpdateMatchUseCase updateMatchUseCase,
    IDeleteMatchUseCase deleteMatchUseCase)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetMatches([FromQuery] MatchParameters matchParameters, CancellationToken cancellationToken)
    {
        var pagedResult = await getAllMatchesUseCase.ExecuteAsync(matchParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.matches);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetMatchById(string id, CancellationToken cancellationToken)
    {
        var matchToGet = await getMatchByIdUseCase.ExecuteAsync(id, cancellationToken);

        return Ok(matchToGet);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMatch([FromBody] MatchCreateDto matchCreateDto, CancellationToken cancellationToken)
    {
        await createMatchUseCase.ExecuteAsync(matchCreateDto, cancellationToken);

        return StatusCode(201);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMatch(string id, [FromBody] MatchUpdateDto matchUpdateDto, CancellationToken cancellationToken)
    {
        await updateMatchUseCase.ExecuteAsync(id, matchUpdateDto, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMatch(string id, CancellationToken cancellationToken)
    {
        await deleteMatchUseCase.ExecuteAsync(id, cancellationToken);

        return NoContent();
    }
}