using SportDataService.Application.DTO.Event;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.Contracts.UseCases.Event;

public interface IGetAllEventsUseCase
{
    Task<(IEnumerable<EventGetDto> events, MetaData metaData)> ExecuteAsync(EventParameters eventParameters, CancellationToken cancellationToken);
}