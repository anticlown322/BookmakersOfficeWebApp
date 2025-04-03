using SportDataService.Application.DTO.Event;

namespace SportDataService.Application.Contracts.UseCases.Event;

public interface ICreateEventUseCase
{
    Task<EventGetDto> ExecuteAsync(EventCreateDto eventCreateDto, CancellationToken cancellationToken);
}