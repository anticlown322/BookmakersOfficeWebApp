using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Event;
using SportDataService.Application.DTO.Event;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Event;

public sealed class UpdateEventUseCase(
    IEventRepository eventRepository,
    IMapper mapper)
    : IUpdateEventUseCase
{
    public async Task ExecuteAsync(string eventId, EventUpdateDto eventUpdateDto, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(eventId, out _))
        {
            throw new ArgumentException("Invalid Event ID format.");
        }

        var eventToUpdate = await eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (eventToUpdate == null)
        {
            throw new EventNotFoundByIdException(eventId);
        }

        mapper.Map(eventUpdateDto, eventToUpdate);
        await eventRepository.UpdateAsync(eventToUpdate, cancellationToken);
    }
}