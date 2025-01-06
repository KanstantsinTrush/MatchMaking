using MatchMaking.Worker.BLCore.Services;

namespace MatchMaking.Worker.BackgroundServices;

public class MatchMakerService(
    IServiceScopeFactory serviceProvider,
    ILogger<MatchMakerService> logger) : BackgroundService
{
    private readonly TimeSpan _delayOnException = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var matchMakingService = scope.ServiceProvider.GetRequiredService<IMatchMakingService>();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await matchMakingService.ProcessMatch();

                await Task.Delay(100, stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);

                await Task.Delay(_delayOnException, stoppingToken);
            }
        }
    }
}