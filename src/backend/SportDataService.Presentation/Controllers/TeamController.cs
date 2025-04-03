using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SportDataService.Application.Contracts.UseCases.Team;
using SportDataService.Application.DTO.Team;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Presentation.Controllers;

[Route("api/teams")]
[ApiController]
public class TeamController(
    IGetAllTeamsUseCase getAllTeamsUseCase,
    IGetTeamByIdUseCase getTeamByIdUseCase,
    ICreateTeamUseCase createTeamUseCase,
    IUpdateTeamUseCase updateTeamUseCase,
    IDeleteTeamUseCase deleteTeamUseCase)
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

    [HttpPost]
    public async Task<IActionResult> CreateTeam([FromBody] TeamCreateDto teamCreateDto, CancellationToken cancellationToken)
    {
        await createTeamUseCase.ExecuteAsync(teamCreateDto, cancellationToken);

        return StatusCode(201);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTeam(string id, [FromBody] TeamUpdateDto teamUpdateDto, CancellationToken cancellationToken)
    {
        await updateTeamUseCase.ExecuteAsync(id, teamUpdateDto, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTeam(string id, CancellationToken cancellationToken)
    {
        await deleteTeamUseCase.ExecuteAsync(id, cancellationToken);

        return NoContent();
    }
}