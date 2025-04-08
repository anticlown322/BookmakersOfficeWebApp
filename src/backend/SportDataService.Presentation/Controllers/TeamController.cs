using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SportDataService.Application.Contracts.UseCases.Team;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Presentation.Controllers;

[Route("api/teams")]
[ApiController]
public class TeamController(
    IGetAllTeamsUseCase getAllTeamsUseCase,
    IGetTeamByIdUseCase getTeamByIdUseCase,
    IGetTeamByTeamIdUseCase getTeamByTeamIdUseCase)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTeams([FromQuery] TeamParameters teamParameters, CancellationToken cancellationToken)
    {
        var pagedResult = await getAllTeamsUseCase.ExecuteAsync(teamParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.teams);
    }

    [HttpGet("by-id/{id}")]
    public async Task<IActionResult> GetTeamById(string id, CancellationToken cancellationToken)
    {
        var teamToGet = await getTeamByIdUseCase.ExecuteAsync(id, cancellationToken);

        return Ok(teamToGet);
    }

    [HttpGet("by-team-id/{teamId}")]
    public async Task<IActionResult> GetTeamByTeamId(string teamId, CancellationToken ct)
    {
        var team = await getTeamByTeamIdUseCase.ExecuteAsync(teamId, ct);
        return Ok(team);
    }
}