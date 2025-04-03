using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SportDataService.Application.Contracts.UseCases.League;
using SportDataService.Application.DTO.League;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Presentation.Controllers;

[Route("api/leagues")]
[ApiController]
public class LeagueController(
    IGetAllLeaguesUseCase getAllLeaguesUseCase,
    IGetLeagueByIdUseCase getLeagueByIdUseCase,
    ICreateLeagueUseCase createLeagueUseCase,
    IUpdateLeagueUseCase updateLeagueUseCase,
    IDeleteLeagueUseCase deleteLeagueUseCase)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetLeagues([FromQuery] LeagueParameters leagueParameters, CancellationToken cancellationToken)
    {
        var pagedResult = await getAllLeaguesUseCase.ExecuteAsync(leagueParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.leagues);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLeagueById(string id, CancellationToken cancellationToken)
    {
        var leagueToGet = await getLeagueByIdUseCase.ExecuteAsync(id, cancellationToken);

        return Ok(leagueToGet);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLeague([FromBody] LeagueCreateDto leagueCreateDto, CancellationToken cancellationToken)
    {
        await createLeagueUseCase.ExecuteAsync(leagueCreateDto, cancellationToken);

        return StatusCode(201);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLeague(string id, [FromBody] LeagueUpdateDto leagueUpdateDto, CancellationToken cancellationToken)
    {
        await updateLeagueUseCase.ExecuteAsync(id, leagueUpdateDto, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteLeague(string id, CancellationToken cancellationToken)
    {
        await deleteLeagueUseCase.ExecuteAsync(id, cancellationToken);

        return NoContent();
    }
}