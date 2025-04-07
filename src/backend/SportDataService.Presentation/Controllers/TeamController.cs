using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SportDataService.Application.Contracts.UseCases.Team;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Presentation.Controllers;

[Route("api/teams")]
[ApiController]
public class TeamController(
    IGetAllTeamsUseCase getAllTeamsUseCase,
    IGetTeamByIdUseCase getTeamByIdUseCase)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTeams([FromQuery] TeamParameters teamParameters, CancellationToken cancellationToken)
    {
        var pagedResult = await getAllTeamsUseCase.ExecuteAsync(teamParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.teams);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTeamById(string id, CancellationToken cancellationToken)
    {
        var teamToGet = await getTeamByIdUseCase.ExecuteAsync(id, cancellationToken);

        return Ok(teamToGet);
    }
}