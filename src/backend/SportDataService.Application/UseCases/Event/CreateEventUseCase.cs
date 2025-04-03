using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Event;
using SportDataService.Application.DTO.Event;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Event;

public sealed class CreateEventUseCase(
    IEventRepository eventRepository,
    IMapper mapper) : ICreateEventUseCase
{
    public async Task<EventGetDto> ExecuteAsync(EventCreateDto eventCreateDto, CancellationToken cancellationToken)
    {
        var eventToCreate = mapper.Map<Domain.Models.Event>(eventCreateDto);
        eventToCreate.Id = ObjectId.GenerateNewId().ToString();

        cancellationToken.ThrowIfCancellationRequested();

        await eventRepository.CreateAsync(eventToCreate, cancellationToken);

        var eventGetDto = mapper.Map<EventGetDto>(eventToCreate);
        return eventGetDto;
    }
}