namespace SportDataService.Application.Contracts.UseCases.Event;

public interface IDeleteEventUseCase
{
    Task ExecuteAsync(string eventId, CancellationToken cancellationToken);
}