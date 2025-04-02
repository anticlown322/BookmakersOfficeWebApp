namespace SportDataService.Application.Contracts.Services;

public interface ILoggerService
{
    void LogInfo(string message);
    void LogWarn(string message);
    void LogDebug(string message);
    void LogError(string message);
}
