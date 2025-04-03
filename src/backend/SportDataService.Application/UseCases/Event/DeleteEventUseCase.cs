using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Event;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Event;

public sealed class DeleteEventUseCase(
    IEventRepository eventRepository)
    : IDeleteEventUseCase
{
    public async Task ExecuteAsync(string eventId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(eventId, out _))
        {
            throw new ArgumentException("Invalid Event ID format.");
        }

        var eventToDelete = await eventRepository.GetByIdAsync(eventId, cancellationToken);
        if (eventToDelete == null)
        {
            throw new EventNotFoundByIdException(eventId);
        }

        await eventRepository.DeleteAsync(eventId, cancellationToken);
    }
}