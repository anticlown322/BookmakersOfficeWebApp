using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SportDataService.Application.Contracts.UseCases.Event;
using SportDataService.Application.DTO.Event;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Presentation.Controllers;

[Route("api/events")]
[ApiController]
public class EventController(
    IGetAllEventsUseCase getAllEventsUseCase,
    IGetEventByIdUseCase getEventByIdUseCase,
    ICreateEventUseCase createEventUseCase,
    IUpdateEventUseCase updateEventUseCase,
    IDeleteEventUseCase deleteEventUseCase)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetEvents([FromQuery] EventParameters eventParameters, CancellationToken cancellationToken)
    {
        var pagedResult = await getAllEventsUseCase.ExecuteAsync(eventParameters, cancellationToken);

        Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));

        return Ok(pagedResult.events);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById(string id, CancellationToken cancellationToken)
    {
        var eventToGet = await getEventByIdUseCase.ExecuteAsync(id, cancellationToken);

        return Ok(eventToGet);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] EventCreateDto eventCreateDto, CancellationToken cancellationToken)
    {
        await createEventUseCase.ExecuteAsync(eventCreateDto, cancellationToken);

        return StatusCode(201);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEvent(string id, [FromBody] EventUpdateDto eventUpdateDto, CancellationToken cancellationToken)
    {
        await updateEventUseCase.ExecuteAsync(id, eventUpdateDto, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(string id, CancellationToken cancellationToken)
    {
        await deleteEventUseCase.ExecuteAsync(id, cancellationToken);

        return NoContent();
    }
}