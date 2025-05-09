using BettingService.BLL.Contracts.Services;
using Microsoft.Extensions.Hosting;

namespace BettingService.BLL.Services.Kafka;

public class KafkaBackgroundService(
    IKafkaProducerService producerService,
    IKafkaConsumerService consumerService)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (producerService is IDisposable producerDisposable)
        {
            producerDisposable.Dispose();
        }

        if (consumerService is IDisposable consumerDisposable)
        {
            consumerDisposable.Dispose();
        }

        await Task.CompletedTask;
    }
}