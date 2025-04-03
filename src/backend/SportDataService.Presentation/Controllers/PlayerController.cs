using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SportDataService.Application.Contracts.UseCases.Player;
using SportDataService.Application.DTO.Player;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Presentation.Controllers;

[Route("api/players")]
[ApiController]
public class PlayerController(
    IGetAllPlayersUseCase getAllPlayersUseCase,
    IGetPlayerByIdUseCase getPlayerByIdUseCase,
    ICreatePlayerUseCase createPlayerUseCase,
    IUpdatePlayerUseCase updatePlayerUseCase,
    IDeletePlayerUseCase deletePlayerUseCase)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetPlayers([FromQuery] PlayerParameters playerParameters, CancellationToken cancellationToken)
    {
        var pagedResult = await getAllPlayersUseCase.ExecuteAsync(playerParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.players);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPlayerById(string id, CancellationToken cancellationToken)
    {
        var playerToGet = await getPlayerByIdUseCase.ExecuteAsync(id, cancellationToken);

        return Ok(playerToGet);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePlayer([FromBody] PlayerCreateDto playerCreateDto, CancellationToken cancellationToken)
    {
        await createPlayerUseCase.ExecuteAsync(playerCreateDto, cancellationToken);

        return StatusCode(201);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePlayer(string id, [FromBody] PlayerUpdateDto playerUpdateDto, CancellationToken cancellationToken)
    {
        await updatePlayerUseCase.ExecuteAsync(id, playerUpdateDto, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayer(string id, CancellationToken cancellationToken)
    {
        await deletePlayerUseCase.ExecuteAsync(id, cancellationToken);

        return NoContent();
    }
}