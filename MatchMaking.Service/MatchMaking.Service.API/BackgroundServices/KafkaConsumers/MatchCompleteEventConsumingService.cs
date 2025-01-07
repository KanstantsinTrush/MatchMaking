using System.Diagnostics;
using System.Text.Json;
using MatchMaking.Infrastructure.Kafka.Configuration;
using MatchMaking.Infrastructure.Kafka.Consumers;
using MatchMaking.Service.DBEntites;
using MatchMaking.Service.Services;
using Microsoft.Extensions.Options;

namespace MatchMaking.Service.API.BackgroundServices.KafkaConsumers;

public class MatchCompleteEventConsumingService(
    IServiceScopeFactory serviceProvider,
    IKafkaConsumerFactory consumerFactory,
    IOptions<KafkaConsumerOptions> consumerOptions,
    ILogger<MatchCompleteEventConsumingService> logger) : BackgroundService
{
    private readonly string _topic = consumerOptions.Value.MatchMakingCompleteEvents;
    private readonly TimeSpan _delayOnException = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumer = consumerFactory.Create(_topic, logger);

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        await using var scope = serviceProvider.CreateAsyncScope();
                        var matchMakingService = scope.ServiceProvider.GetRequiredService<IMatchMakingService>();

                        var result = consumer.Consume(stoppingToken);

                        var evt = JsonSerializer.Deserialize<Match>(result.Message.Value);

                        if (evt is null)
                        {
                            throw new InvalidOperationException("Deserialized event is null.");
                        }

                        await matchMakingService.ProcessMatch(evt);
                    }
                }
                finally
                {
                    consumer.Close();
                }
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);

                await Task.Delay(_delayOnException, stoppingToken);
            }
        }
    }
}