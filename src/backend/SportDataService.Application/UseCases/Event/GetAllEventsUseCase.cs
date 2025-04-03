using AutoMapper;
using SportDataService.Application.Contracts.UseCases.Event;
using SportDataService.Application.DTO.Event;
using SportDataService.Application.DTO.Event;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.UseCases.Event;

public sealed class GetAllEventsUseCase(
    IEventRepository eventRepository,
    IMapper mapper)
    : IGetAllEventsUseCase
{
    public async Task<(IEnumerable<EventGetDto> events, MetaData metaData)> ExecuteAsync(EventParameters eventParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var eventsWithMetaData = await eventRepository.FindAllEventsAsync(eventParameters, cancellationToken);

        var eventGetDtos = mapper.Map<IEnumerable<EventGetDto>>(eventsWithMetaData);

        return (
            events: eventGetDtos,
            metaData: eventsWithMetaData.MetaData);
    }
}