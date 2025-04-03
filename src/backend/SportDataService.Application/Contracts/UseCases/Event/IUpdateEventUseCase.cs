using SportDataService.Application.DTO.Event;

namespace SportDataService.Application.Contracts.UseCases.Event;

public interface IUpdateEventUseCase
{
    Task ExecuteAsync(string eventId, EventUpdateDto eventUpdateDto, CancellationToken cancellationToken);
}