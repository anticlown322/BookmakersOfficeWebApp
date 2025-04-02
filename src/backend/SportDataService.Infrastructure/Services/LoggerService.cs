using NLog;
using SportDataService.Application.Contracts.Services;

namespace SportDataService.Infrastructure.Services;

public class LoggerService : ILoggerService
{
    private static ILogger _logger = LogManager.GetCurrentClassLogger();

    public void LogDebug(string message) => _logger.Debug(message);
    public void LogError(string message) => _logger.Error(message);
    public void LogInfo(string message) => _logger.Info(message);
    public void LogWarn(string message) => _logger.Warn(message);
}