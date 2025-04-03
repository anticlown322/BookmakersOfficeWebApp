using SportDataService.Application.DTO.Event;

namespace SportDataService.Application.Contracts.UseCases.Event;

public interface IGetEventByIdUseCase
{
    Task<EventGetDto> ExecuteAsync(string eventId, CancellationToken cancellationToken);
}