using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Event;
using SportDataService.Application.DTO.Event;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Event;

public sealed class GetEventByIdUseCase(
    IEventRepository eventRepository,
    IMapper mapper)
    : IGetEventByIdUseCase
{
    public async Task<EventGetDto> ExecuteAsync(string eventId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(eventId, out _))
        {
            throw new ArgumentException("Invalid Event ID format.");
        }

        var eventToGet = await eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (eventToGet == null)
        {
            throw new EventNotFoundByIdException(eventId);
        }

        return mapper.Map<EventGetDto>(eventToGet);
    }
}