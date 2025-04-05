using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SportDataService.Application.Contracts.UseCases.Odds;
using SportDataService.Application.DTO.Odds;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Presentation.Controllers;

[Route("api/odds")]
[ApiController]
public class OddsController(
    IGetAllOddsUseCase getAllOddsUseCase,
    IGetOddsByIdUseCase getOddsByIdUseCase,
    ICreateOddsUseCase createOddsUseCase,
    IUpdateOddsUseCase updateOddsUseCase,
    IDeleteOddsUseCase deleteOddsUseCase)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetOdds([FromQuery] OddsParameters oddsParameters, CancellationToken cancellationToken)
    {
        var pagedResult = await getAllOddsUseCase.ExecuteAsync(oddsParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.odds);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOddsById(string id, CancellationToken cancellationToken)
    {
        var oddsToGet = await getOddsByIdUseCase.ExecuteAsync(id, cancellationToken);

        return Ok(oddsToGet);
    }

    [HttpPost]
    public async Task<IActionResult> CreateOdds([FromBody] OddsCreateDto oddsCreateDto, CancellationToken cancellationToken)
    {
        await createOddsUseCase.ExecuteAsync(oddsCreateDto, cancellationToken);

        return StatusCode(201);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOdds(string id, [FromBody] OddsUpdateDto oddsUpdateDto, CancellationToken cancellationToken)
    {
        await updateOddsUseCase.ExecuteAsync(id, oddsUpdateDto, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOdds(string id, CancellationToken cancellationToken)
    {
        await deleteOddsUseCase.ExecuteAsync(id, cancellationToken);

        return NoContent();
    }
}