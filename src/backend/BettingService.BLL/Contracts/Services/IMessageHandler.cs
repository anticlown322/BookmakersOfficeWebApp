namespace BettingService.BLL.Contracts.Services;

public interface IMessageHandler<T>
{
    Task HandleAsync(string message, CancellationToken ct);
}