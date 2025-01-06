using MatchMaking.Infrastructure.Kafka.Configuration;
using MatchMaking.Infrastructure.Kafka.Consumers;
using MatchMaking.Worker.BLCore.Services;
using Microsoft.Extensions.Options;

namespace MatchMaking.Worker;

public class MatchMakingRequestEventConsumingService(
    IServiceScopeFactory serviceProvider,
    IKafkaConsumerFactory consumerFactory,
    IOptions<KafkaConsumerOptions> consumerOptions,
    ILogger<MatchMakingRequestEventConsumingService> logger)
    : BackgroundService
{
    private readonly string _topic = consumerOptions.Value.MatchMakingRequestEvents;
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
                        using var scope = serviceProvider.CreateScope();
                        var matchMakingService = scope.ServiceProvider.GetRequiredService<IMatchMakingService>();
                        
                        var result = consumer.Consume(stoppingToken);
                        
                        var userId = result.Message.Value;
                        
                        logger.LogInformation("User is received: {UserId}", userId);

                        await matchMakingService.ProcessUser(userId);
                        
                        consumer.Commit(result);
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